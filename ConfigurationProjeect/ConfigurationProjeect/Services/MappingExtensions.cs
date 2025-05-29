using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViralWave.Application.DTOs;
using ViralWave.Application.Interfaces.Implementation;
using ViralWave.Domain.Entities;
using ViralWave.Persistence.Context;

namespace ViralWave.Infrastructure.Extensions
{
    public static class MappingExtensions
    {
        public static DistributedBatchDTO DistributedBatchDTO(this IEnumerable<DistributedBatchResult> batches, IContratoService _contratoService)
        {
            var distributedBatchesDTO = new DistributedBatchDTO();

            foreach (var batch in batches)
            {

                var cp = _contratoService.GetParty(batch.PartyId ?? 0).Result;
                var distributedDto = new DistributedModel
                {
                    AccountManager = batch.AccountManager,
                    BatchId = batch.BatchId,
                    ReleaseDate = batch.ReleaseDate,
                    UPC = batch.UPC,
                    SingerName = batch.SingerName,
                    CP = cp?.FirstOrDefault()?.Name
                };
                distributedBatchesDTO.distBatch.Add(distributedDto);
            }

            return distributedBatchesDTO;
        }
        public static PendingBatchDTO PendingBatchDTO(this IEnumerable<PendingBatchResult> batches)
        {
            PendingBatchDTO pendingBatchesDTO = new();


            foreach (var batch in batches)
            {
                var pendingBathcModel = new PendingBatchModel
                {
                    AccountManager = batch.AccountManager,
                    BatchId = batch.BatchId,
                    ReleaseTitle = batch.ReleaseTitle,
                    ArtistName = batch.ArtistName,
                    ReleaseDate = batch.ReleaseDate
                };

                pendingBatchesDTO.pendingBatchList.Add(pendingBathcModel);
            }

            return pendingBatchesDTO;
        }

        public static List<ValidContractsDto> ValidContractsDto(this IEnumerable<Get_Provider_Contract_Per_Month_Per_YearResult> contracts)
        {
            return contracts.AsEnumerable().Select(c => new ValidContractsDto
            {
                Id = c.ID,
                Name = c.Name,
                Countries = c.Countries,
                DocId = c.Doc_ID ?? 0,
                EndDate = c.End_Date,
                IsAnnex = c.Is_Annex,
                IsAutoRenewal = c.Is_AutoRenewal,
                IsExpired = c.Is_Expired,
                IsMuzicUp = c.Is_MuzicUP,
                LegalName = c.Legal_Name,
                Operators = c.Operators,
                Path = c.path,
                ProviderDataId = c.Provider_Data_ID,
                RevenueShare = Convert.ToDecimal(c.Revenue_Share),
                StartDate = c.Start_Date
            }
            ).ToList();
        }

        public static List<ProviderDto> ProviderDto(this IEnumerable<Get_Provider_List_Legal_Per_Interval_Per_ClientResult> providers)
        {
            return providers.AsEnumerable().Select(p => new ProviderDto
            {
                Id = p.ID ?? 0,
                Name = p.Name
            }).ToList();
        }

        public static List<FinancialRevenueReportDto> FinancialRevenueReportDto(this IEnumerable<Proc_Report_FinancialResult> financialResults)
        {
            return financialResults.Select(f => new FinancialRevenueReportDto
            {
                Color = f.Color,
                Cost = f.Cost ?? 0,
                Country = f.Country,
                Date = f.Month_Date?.ToString("d") ?? default,
                PlatformName = f.PlatfromName,
                Rev = f.TotalRevenue ?? 0
            }).ToList();
        }

    }
}
