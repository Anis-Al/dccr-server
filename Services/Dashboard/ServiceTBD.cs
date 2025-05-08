using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

                foreach (var kpi in kpis)
                {
                    if (string.IsNullOrEmpty(kpi.requete_sql))
                    {
                        _logger.LogWarning($"KPI {kpi.id_kpi} has empty SQL query");
                        continue;
                    }

                    try
                    {
                        if (!IsValidSqlQuery(kpi.requete_sql))
                        {
                            _logger.LogError($"Invalid SQL query for KPI {kpi.id_kpi}");
                            continue;
                        }

                        var resultats_requete = await _contexte.Database
                            .SqlQuery<dynamic>($"{kpi.requete_sql}").ToListAsync();

                        if (resultats_requete == null || !resultats_requete.Any())
                        {
                            _logger.LogInformation($"No results found for KPI {kpi.id_kpi}");
                        }

                        resultats.Add(new ResultatDTO<dynamic>
                        {
                            id_kpi = kpi.id_kpi,
                            description_kpi = kpi.description_kpi ?? string.Empty,
                            resultats = resultats_requete ?? Enumerable.Empty<dynamic>()
                        });
                    }
                    catch (InvalidOperationException ex)
                    {
                        _logger.LogError(ex, $"Invalid operation for KPI {kpi.id_kpi}: {ex.Message}");
                        continue;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error executing query for KPI {kpi.id_kpi}: {ex.Message}");
                        continue;
                    }
                }

                return resultats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ExecuterToutesRequetes");
                throw;
            }
        }

        private bool IsValidSqlQuery(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return false;

            // Basic SQL injection prevention
            var invalidKeywords = new[] { "DROP", "DELETE", "TRUNCATE", "ALTER", "CREATE", "EXEC", "EXECUTE" };
            return !invalidKeywords.Any(keyword => 
                query.ToUpperInvariant().Contains($" {keyword} ") || 
                query.ToUpperInvariant().StartsWith($"{keyword} "));
        }

        public async Task<IEnumerable<TableauDeBord>> GetAllKpisAsync()
        {
            try
            {
                var kpis = await _contexte.tableau_de_bord
                    .AsNoTracking()
                    .ToListAsync();

                if (kpis == null || !kpis.Any())
                {
                    _logger.LogWarning("No KPIs found in the database");
                    return Enumerable.Empty<TableauDeBord>();
                }

                return kpis;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Database operation error in GetAllKpisAsync");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetAllKpisAsync");
                throw;
            }
        }
    }
}