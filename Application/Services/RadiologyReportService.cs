
using Application.Common;
using Application.DTOs.RequestDto;
using Application.DTOs.ResponseDto;
using Application.Interfaces.IRepositories;
using Application.Interfaces.IServices;
using Application.Specifications;
using Domain.Entities;
using Domain.Enums;
using Domain.Specifications;
using static System.Net.WebRequestMethods;

namespace Application.Services
{
    public class RadiologyReportService : IRadiologyReportService
    {
        private const int MaxPdfSizeBytes = 10 * 1024 * 1024; // 10 MB

        private readonly IUnitOfWork _unitOfWork;
        private readonly IPdfGenerationService _pdfGenerationService;

        public RadiologyReportService(
            IUnitOfWork unitOfWork,
            IPdfGenerationService pdfGenerationService)
        {
            _unitOfWork = unitOfWork;
            _pdfGenerationService = pdfGenerationService;
        }


        // ---------- Create ----------
        public async Task CreateAsync(
            CreateRadiologyReportDto dto, int createdByUserId)
        {
            if (dto.Fields == null || dto.Fields.Count == 0)
                throw new ArgumentException("Report fields are required.");

            var study = await _unitOfWork.Repository<RadiologyStudy>()
                .GetByIdAsync(dto.StudyId)
                ?? throw new KeyNotFoundException(
                    $"Radiology study {dto.StudyId} was not found.");

            if (study.Status == StudyStatus.Cancelled)
                throw new InvalidOperationException(
                    "Cannot create a report for a cancelled study.");

            // Reports are written by radiologists
            var isActiveRadiologist =
                await _unitOfWork.Repository<Radiologist>()
                    .AnyAsync(r =>
                        r.UserId == createdByUserId && r.User.IsActive);

            if (!isActiveRadiologist)
                throw new KeyNotFoundException(
                    $"Active radiologist with user id {createdByUserId} was not found.");

            // 1:1 relationship: one report per study
            var alreadyReported =
                await _unitOfWork.Repository<RadiologyReport>()
                    .AnyAsync(r => r.StudyId == dto.StudyId);

            if (alreadyReported)
                throw new InvalidOperationException(
                    $"Study {dto.StudyId} already has a report.");

            // Generate PDF
            var pdfBytes = _pdfGenerationService.GeneratePdf(dto.Fields);

            ValidatePdf(pdfBytes);

            var report = new RadiologyReport
            {
                StudyId = dto.StudyId,
                CreatedByUserId = createdByUserId,
                PdfFile = pdfBytes
            };

            await _unitOfWork.Repository<RadiologyReport>()
                .AddAsync(report);

            await _unitOfWork.SaveChangesAsync();
        }

        // ---------- Read ----------
        public async Task<PagedResult<RadiologyReportResponseDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            var spec = new RadiologyReportWithIncludeSpecification(
                pageNumber, pageSize);

            return await ListPagedAsync(spec);
        }
        public async Task<PagedResult<RadiologyReportResponseDto>> GetAllMyReportsAsync(int pageNumber, int pageSize,int userId)
        {
            var filter = new RadiologyReportSearchDto{ PageNumber = pageNumber, PageSize = pageSize, UserId = userId };
            var spec = new RadiologyReportSearchSpecification(filter);
            return await ListPagedAsync(spec);
        }
        public async Task<RadiologyReportResponseDto> GetByIdAsync(
            int reportId)
        {
            RadiologyReportSearchDto filter = new();
            filter.ReportId = reportId;

            var reports = await _unitOfWork.Repository<RadiologyReport>()
                .ListAsync(new RadiologyReportSearchSpecification(filter));

            var report = reports.FirstOrDefault()
                ?? throw new KeyNotFoundException(
                    $"Radiology report {reportId} was not found.");

            return ToDto(report);
        }

        public async Task<byte[]> GetPdfAsync(int reportId)
        {
            var report = await GetTrackedReportAsync(reportId);
            return report.PdfFile;
        }
        public async Task<byte[]> GetMyPdfAsync(int reportId,int userId)
        {
            var report = await _unitOfWork.Repository<RadiologyReport>().FirstOrDefaultAsync
                (r=> r.Id== reportId && r.Study.Order.Patient.UserId==userId);
            if(report == null)
            {
                throw new KeyNotFoundException($"Radiology report {reportId} was not found.");
            }
            return report.PdfFile;
        }

        public async Task<PagedResult<RadiologyReportResponseDto>> SearchAsync(
            RadiologyReportSearchDto filter)
        {
            var spec = new RadiologyReportSearchSpecification(filter);
            return await ListPagedAsync(spec);
        }
        public async Task<PagedResult<RadiologyReportResponseDto>> SearchMyReportsAsync(
            RadiologyReportSearchDto filter,int userId)
        {
            filter.UserId = userId;
            var spec = new RadiologyReportSearchSpecification(filter);
            return await ListPagedAsync(spec);
        }

        // ---------- Update ----------
        public async Task UpdateAsync(
            int reportId, UpdateRadiologyReportDto dto)
        {
            if (dto.Fields == null || dto.Fields.Count == 0)
                throw new ArgumentException("Report fields are required.");

            var report = await GetTrackedReportAsync(reportId);

            // Generate new PDF from updated fields
            var pdfBytes = _pdfGenerationService.GeneratePdf(dto.Fields);

            ValidatePdf(pdfBytes);

            report.PdfFile = pdfBytes;

            _unitOfWork.Repository<RadiologyReport>().Update(report);

            await _unitOfWork.SaveChangesAsync();

          
        }

        // ---------- Delete ----------
        public async Task DeleteAsync(int reportId)
        {
            var report = await GetTrackedReportAsync(reportId);

            _unitOfWork.Repository<RadiologyReport>().Delete(report);

            await _unitOfWork.SaveChangesAsync();
        }

        // ---------- Helpers ----------
        private async Task<RadiologyReport> GetTrackedReportAsync(
            int reportId)
        {
            return await _unitOfWork.Repository<RadiologyReport>()
                .GetByIdAsync(reportId)
                ?? throw new KeyNotFoundException(
                    $"Radiology report {reportId} was not found.");
        }

        private async Task<PagedResult<RadiologyReportResponseDto>>
            ListPagedAsync(ISpecification<RadiologyReport> spec)
        {
            var repo = _unitOfWork.Repository<RadiologyReport>();

            var items = await repo.ListAsync(spec);
            var total = await repo.CountAsync(spec);

            return new PagedResult<RadiologyReportResponseDto>
            {
                Items = items.Select(ToDto).ToList(),
                TotalCount = total
            };
        }

        private static void ValidatePdf(byte[]? pdf)
        {
            if (pdf is null || pdf.Length == 0)
                throw new ArgumentException("PDF file is required.");

            if (pdf.Length > MaxPdfSizeBytes)
                throw new ArgumentException(
                    "PDF file must not exceed 10 MB.");

            // Every PDF starts with the bytes "%PDF-"
            if (pdf.Length < 5 ||
                pdf[0] != 0x25 ||
                pdf[1] != 0x50 ||
                pdf[2] != 0x44 ||
                pdf[3] != 0x46 ||
                pdf[4] != 0x2D)
            {
                throw new ArgumentException(
                    "The generated file is not a valid PDF.");
            }
        }

        private static RadiologyReportResponseDto ToDto(
            RadiologyReport r) => new()
            {
                ReportId = r.Id,
                StudyId = r.StudyId,
                StudyInstanceUID = r.Study?.StudyInstanceUID ?? string.Empty,
                CreatedByUserId = r.CreatedByUserId,
                CreatedByName = r.CreatedByUser?.FullName ?? string.Empty
            };
    }
}
