using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper; 
using DCCR_SERVER.Context;
using DCCR_SERVER.DTOs.Dashboard;
using DCCR_SERVER.Models.Principaux;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DCCR_SERVER.Services.Dashboard
{
    public class ServiceTBD
    {
        private readonly BddContext _contexte;
        private readonly ILogger<ServiceTBD> _logger;

        public ServiceTBD(BddContext contexte, ILogger<ServiceTBD> logger)
        {
            _contexte = contexte;
            _logger = logger;
        }
        public async Task<IEnumerable<ResultatDTO<dynamic>>> ExecuterToutesRequetes()
        {
            try
            {
                var kpis = await GetAllKpisAsync();
                var resultats = new List<ResultatDTO<dynamic>>();

                var connection = _contexte.Database.GetDbConnection();

                foreach (var kpi in kpis)
                {
                    if (string.IsNullOrEmpty(kpi.requete_sql))
                    {
                        _logger.LogWarning($"KPI {kpi.id_kpi} ({kpi.description_kpi}) has an empty SQL query. Skipping.");
                        continue;
                    }

                    try
                    {
                        var resultats_requete = await connection.QueryAsync<dynamic>(kpi.requete_sql);

                        resultats.Add(new ResultatDTO<dynamic>
                        {
                            id_kpi = kpi.id_kpi,
                            description_kpi = kpi.description_kpi ?? string.Empty,
                            resultats = resultats_requete ?? Enumerable.Empty<dynamic>()
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $" {kpi.id_kpi} ({kpi.description_kpi}): {ex.Message}");
                        continue;
                    }
                }

                return resultats;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<IEnumerable<TableauDeBord>> GetAllKpisAsync()
        {
            return await _contexte.tableau_de_bord.AsNoTracking().ToListAsync();
        }
    }
}