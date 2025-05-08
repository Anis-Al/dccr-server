using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DCCR_SERVER.Migrations
{
    /// <inheritdoc />
    public partial class creation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "activites_credit",
                columns: table => new
                {
                    code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    domaine = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_activites_credit", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "agences",
                columns: table => new
                {
                    code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    domaine = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_agences", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "classes_retard",
                columns: table => new
                {
                    code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    domaine = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_classes_retard", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "durees_credit",
                columns: table => new
                {
                    code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    domaine = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_durees_credit", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "intervenants",
                columns: table => new
                {
                    cle = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    type_cle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    nif = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cli = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    rib = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_intervenants", x => x.cle);
                });

            migrationBuilder.CreateTable(
                name: "mapping_colonnes",
                columns: table => new
                {
                    id_mapping = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    colonne_excel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    colonne_bdd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    colonne_prod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    table_prod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    type_donnee_prod = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mapping_colonnes", x => x.id_mapping);
                });

            migrationBuilder.CreateTable(
                name: "monnaies",
                columns: table => new
                {
                    code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    domaine = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_monnaies", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "niveaux_responsabilite",
                columns: table => new
                {
                    code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    domaine = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_niveaux_responsabilite", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "parametrage_fichiers_xml",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    parametre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    valeur = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parametrage_fichiers_xml", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "pays",
                columns: table => new
                {
                    code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    domaine = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pays", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "regles_validation",
                columns: table => new
                {
                    id_regle = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nom_colonne = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    type_regle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    valeur_regle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    message_erreur = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    colonne_dependante = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    valeur_dependante = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    colonne_cible = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    valeur_cible_attendue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_regles_validation", x => x.id_regle);
                });

            migrationBuilder.CreateTable(
                name: "situations_credit",
                columns: table => new
                {
                    code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    domaine = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_situations_credit", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "tableau_de_bord",
                columns: table => new
                {
                    id_kpi = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    description_kpi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    requete_sql = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tableau_de_bord", x => x.id_kpi);
                });

            migrationBuilder.CreateTable(
                name: "types_credit",
                columns: table => new
                {
                    code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    domaine = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_types_credit", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "types_garantie",
                columns: table => new
                {
                    code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    domaine = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_types_garantie", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "utilisateurs",
                columns: table => new
                {
                    matricule = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    nom_complet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    mot_de_passe = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_utilisateurs", x => x.matricule);
                });

            migrationBuilder.CreateTable(
                name: "wilayas",
                columns: table => new
                {
                    code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    domaine = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wilayas", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "fichiers_excel",
                columns: table => new
                {
                    id_fichier_excel = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nom_fichier_excel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    chemin_fichier_excel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    id_integrateur_excel = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    date_heure_integration_excel = table.Column<DateTime>(type: "datetime2", nullable: false),
                    id_session_import = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    statut_import = table.Column<int>(type: "int", nullable: false),
                    message_statut = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    resume_validation = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fichiers_excel", x => x.id_fichier_excel);
                    table.ForeignKey(
                        name: "FK_fichiers_excel_utilisateurs_id_integrateur_excel",
                        column: x => x.id_integrateur_excel,
                        principalTable: "utilisateurs",
                        principalColumn: "matricule",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "pistes_audit",
                columns: table => new
                {
                    id_action = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    matricule_utilisateur = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    table_ciblée = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    id_entité = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    type_action = table.Column<int>(type: "int", nullable: false),
                    date_action = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pistes_audit", x => x.id_action);
                    table.ForeignKey(
                        name: "FK_pistes_audit_utilisateurs_matricule_utilisateur",
                        column: x => x.matricule_utilisateur,
                        principalTable: "utilisateurs",
                        principalColumn: "matricule",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "lieux",
                columns: table => new
                {
                    id_lieu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code_agence = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    code_wilaya = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    code_pays = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lieux", x => x.id_lieu);
                    table.ForeignKey(
                        name: "FK_lieux_agences_code_agence",
                        column: x => x.code_agence,
                        principalTable: "agences",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_lieux_pays_code_pays",
                        column: x => x.code_pays,
                        principalTable: "pays",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_lieux_wilayas_code_wilaya",
                        column: x => x.code_wilaya,
                        principalTable: "wilayas",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "erreurs_fichiers_excel",
                columns: table => new
                {
                    id_erreur = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_excel = table.Column<int>(type: "int", nullable: false),
                    id_regle = table.Column<int>(type: "int", nullable: true),
                    ligne_excel = table.Column<int>(type: "int", nullable: false),
                    message_erreur = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    id_staging_raw_data = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_erreurs_fichiers_excel", x => x.id_erreur);
                    table.ForeignKey(
                        name: "FK_erreurs_fichiers_excel_fichiers_excel_id_excel",
                        column: x => x.id_excel,
                        principalTable: "fichiers_excel",
                        principalColumn: "id_fichier_excel",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_erreurs_fichiers_excel_regles_validation_id_regle",
                        column: x => x.id_regle,
                        principalTable: "regles_validation",
                        principalColumn: "id_regle",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "fichiers_xml",
                columns: table => new
                {
                    id_fichier_xml = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nom_fichier_xml = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    contenu_correction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    contenu_supression = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    id_utilisateur_generateur_xml = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    date_heure_generation_xml = table.Column<DateTime>(type: "datetime2", nullable: false),
                    id_fichier_excel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fichiers_xml", x => x.id_fichier_xml);
                    table.ForeignKey(
                        name: "FK_fichiers_xml_fichiers_excel_id_fichier_excel",
                        column: x => x.id_fichier_excel,
                        principalTable: "fichiers_excel",
                        principalColumn: "id_fichier_excel",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_fichiers_xml_utilisateurs_id_utilisateur_generateur_xml",
                        column: x => x.id_utilisateur_generateur_xml,
                        principalTable: "utilisateurs",
                        principalColumn: "matricule",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "table_intermediaire_traitement",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_import_excel = table.Column<int>(type: "int", nullable: false),
                    id_session_import = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ligne_original = table.Column<int>(type: "int", nullable: false),
                    numero_contrat = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    date_declaration = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    situation_credit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    date_octroi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    date_rejet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    date_expiration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    date_execution = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    duree_initiale = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    duree_restante = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    type_credit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    activite_credit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    monnaie = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    credit_accorde = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    id_plafond = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    taux = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    mensualite = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cout_total_credit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    solde_restant = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    classe_retard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    date_constatation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    nombre_echeances_impayes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    montant_interets_courus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    montant_interets_retard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    montant_capital_retard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    motif = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    participant_cle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    participant_type_cle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    participant_nif = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    participant_cli = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    participant_rib = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    role_niveau_responsabilite = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    type_garantie = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    montant_garantie = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    code_agence = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    code_wilaya = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    code_pays = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    est_valide = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_table_intermediaire_traitement", x => x.id);
                    table.ForeignKey(
                        name: "FK_table_intermediaire_traitement_fichiers_excel_id_import_excel",
                        column: x => x.id_import_excel,
                        principalTable: "fichiers_excel",
                        principalColumn: "id_fichier_excel",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "credits",
                columns: table => new
                {
                    numero_contrat_credit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    date_declaration = table.Column<DateOnly>(type: "date", nullable: false),
                    id_excel = table.Column<int>(type: "int", nullable: false),
                    est_plafond_accorde = table.Column<bool>(type: "bit", nullable: true),
                    situation_credit = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    date_octroi = table.Column<DateOnly>(type: "date", nullable: false),
                    date_rejet = table.Column<DateOnly>(type: "date", nullable: true),
                    date_expiration = table.Column<DateOnly>(type: "date", nullable: false),
                    date_execution = table.Column<DateOnly>(type: "date", nullable: true),
                    duree_initiale = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    duree_restante = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    id_lieu = table.Column<int>(type: "int", nullable: false),
                    type_credit = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    activite_credit = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    monnaie = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    credit_accorde = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    id_plafond = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    taux = table.Column<decimal>(type: "decimal(8,5)", nullable: false),
                    mensualite = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    cout_total_credit = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    solde_restant = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    classe_retard = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    date_constatation = table.Column<DateOnly>(type: "date", nullable: true),
                    nombre_echeances_impayes = table.Column<int>(type: "int", nullable: true),
                    montant_interets_courus = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    montant_interets_retard = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    montant_capital_retard = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    motif = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_credits", x => new { x.numero_contrat_credit, x.date_declaration, x.id_excel });
                    table.ForeignKey(
                        name: "FK_credits_activites_credit_activite_credit",
                        column: x => x.activite_credit,
                        principalTable: "activites_credit",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_credits_classes_retard_classe_retard",
                        column: x => x.classe_retard,
                        principalTable: "classes_retard",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_credits_durees_credit_duree_initiale",
                        column: x => x.duree_initiale,
                        principalTable: "durees_credit",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_credits_durees_credit_duree_restante",
                        column: x => x.duree_restante,
                        principalTable: "durees_credit",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_credits_fichiers_excel_id_excel",
                        column: x => x.id_excel,
                        principalTable: "fichiers_excel",
                        principalColumn: "id_fichier_excel",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_credits_lieux_id_lieu",
                        column: x => x.id_lieu,
                        principalTable: "lieux",
                        principalColumn: "id_lieu",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_credits_monnaies_monnaie",
                        column: x => x.monnaie,
                        principalTable: "monnaies",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_credits_situations_credit_situation_credit",
                        column: x => x.situation_credit,
                        principalTable: "situations_credit",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_credits_types_credit_type_credit",
                        column: x => x.type_credit,
                        principalTable: "types_credit",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "garanties",
                columns: table => new
                {
                    id_garantie = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cle_interventant = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    numero_contrat_credit = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    date_declaration = table.Column<DateOnly>(type: "date", nullable: false),
                    id_excel = table.Column<int>(type: "int", nullable: false),
                    type_garantie = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    montant_garantie = table.Column<decimal>(type: "decimal(18,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_garanties", x => x.id_garantie);
                    table.ForeignKey(
                        name: "FK_garanties_credits_numero_contrat_credit_date_declaration_id_excel",
                        columns: x => new { x.numero_contrat_credit, x.date_declaration, x.id_excel },
                        principalTable: "credits",
                        principalColumns: new[] { "numero_contrat_credit", "date_declaration", "id_excel" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_garanties_intervenants_cle_interventant",
                        column: x => x.cle_interventant,
                        principalTable: "intervenants",
                        principalColumn: "cle",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_garanties_types_garantie_type_garantie",
                        column: x => x.type_garantie,
                        principalTable: "types_garantie",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "intervenants_credits",
                columns: table => new
                {
                    id_intervenantcredit = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    numero_contrat_credit = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    date_declaration = table.Column<DateOnly>(type: "date", nullable: false),
                    id_excel = table.Column<int>(type: "int", nullable: false),
                    cle_intervenant = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    niveau_responsabilite = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_intervenants_credits", x => x.id_intervenantcredit);
                    table.ForeignKey(
                        name: "FK_intervenants_credits_credits_numero_contrat_credit_date_declaration_id_excel",
                        columns: x => new { x.numero_contrat_credit, x.date_declaration, x.id_excel },
                        principalTable: "credits",
                        principalColumns: new[] { "numero_contrat_credit", "date_declaration", "id_excel" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_intervenants_credits_intervenants_cle_intervenant",
                        column: x => x.cle_intervenant,
                        principalTable: "intervenants",
                        principalColumn: "cle",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_intervenants_credits_niveaux_responsabilite_niveau_responsabilite",
                        column: x => x.niveau_responsabilite,
                        principalTable: "niveaux_responsabilite",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_activites_credit_code",
                table: "activites_credit",
                column: "code");

            migrationBuilder.CreateIndex(
                name: "IX_agences_code",
                table: "agences",
                column: "code");

            migrationBuilder.CreateIndex(
                name: "IX_classes_retard_code",
                table: "classes_retard",
                column: "code");

            migrationBuilder.CreateIndex(
                name: "IX_credits_activite_credit",
                table: "credits",
                column: "activite_credit");

            migrationBuilder.CreateIndex(
                name: "IX_credits_classe_retard",
                table: "credits",
                column: "classe_retard");

            migrationBuilder.CreateIndex(
                name: "IX_credits_date_declaration",
                table: "credits",
                column: "date_declaration");

            migrationBuilder.CreateIndex(
                name: "IX_credits_duree_initiale",
                table: "credits",
                column: "duree_initiale");

            migrationBuilder.CreateIndex(
                name: "IX_credits_duree_restante",
                table: "credits",
                column: "duree_restante");

            migrationBuilder.CreateIndex(
                name: "IX_credits_id_excel",
                table: "credits",
                column: "id_excel");

            migrationBuilder.CreateIndex(
                name: "IX_credits_id_lieu",
                table: "credits",
                column: "id_lieu");

            migrationBuilder.CreateIndex(
                name: "IX_credits_monnaie",
                table: "credits",
                column: "monnaie");

            migrationBuilder.CreateIndex(
                name: "IX_credits_numero_contrat_credit",
                table: "credits",
                column: "numero_contrat_credit");

            migrationBuilder.CreateIndex(
                name: "IX_credits_situation_credit",
                table: "credits",
                column: "situation_credit");

            migrationBuilder.CreateIndex(
                name: "IX_credits_type_credit",
                table: "credits",
                column: "type_credit");

            migrationBuilder.CreateIndex(
                name: "IX_durees_credit_code",
                table: "durees_credit",
                column: "code");

            migrationBuilder.CreateIndex(
                name: "IX_erreurs_fichiers_excel_id_excel",
                table: "erreurs_fichiers_excel",
                column: "id_excel");

            migrationBuilder.CreateIndex(
                name: "IX_erreurs_fichiers_excel_id_regle",
                table: "erreurs_fichiers_excel",
                column: "id_regle");

            migrationBuilder.CreateIndex(
                name: "IX_fichiers_excel_id_fichier_excel",
                table: "fichiers_excel",
                column: "id_fichier_excel");

            migrationBuilder.CreateIndex(
                name: "IX_fichiers_excel_id_integrateur_excel",
                table: "fichiers_excel",
                column: "id_integrateur_excel");

            migrationBuilder.CreateIndex(
                name: "IX_fichiers_xml_id_fichier_excel",
                table: "fichiers_xml",
                column: "id_fichier_excel");

            migrationBuilder.CreateIndex(
                name: "IX_fichiers_xml_id_fichier_xml",
                table: "fichiers_xml",
                column: "id_fichier_xml");

            migrationBuilder.CreateIndex(
                name: "IX_fichiers_xml_id_utilisateur_generateur_xml",
                table: "fichiers_xml",
                column: "id_utilisateur_generateur_xml");

            migrationBuilder.CreateIndex(
                name: "IX_garanties_cle_interventant",
                table: "garanties",
                column: "cle_interventant");

            migrationBuilder.CreateIndex(
                name: "IX_garanties_date_declaration",
                table: "garanties",
                column: "date_declaration");

            migrationBuilder.CreateIndex(
                name: "IX_garanties_id_excel",
                table: "garanties",
                column: "id_excel");

            migrationBuilder.CreateIndex(
                name: "IX_garanties_id_garantie",
                table: "garanties",
                column: "id_garantie");

            migrationBuilder.CreateIndex(
                name: "IX_garanties_numero_contrat_credit",
                table: "garanties",
                column: "numero_contrat_credit");

            migrationBuilder.CreateIndex(
                name: "IX_garanties_numero_contrat_credit_date_declaration_id_excel",
                table: "garanties",
                columns: new[] { "numero_contrat_credit", "date_declaration", "id_excel" });

            migrationBuilder.CreateIndex(
                name: "IX_garanties_type_garantie",
                table: "garanties",
                column: "type_garantie");

            migrationBuilder.CreateIndex(
                name: "IX_intervenants_cle",
                table: "intervenants",
                column: "cle",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_intervenants_credits_cle_intervenant",
                table: "intervenants_credits",
                column: "cle_intervenant");

            migrationBuilder.CreateIndex(
                name: "IX_intervenants_credits_date_declaration",
                table: "intervenants_credits",
                column: "date_declaration");

            migrationBuilder.CreateIndex(
                name: "IX_intervenants_credits_id_excel",
                table: "intervenants_credits",
                column: "id_excel");

            migrationBuilder.CreateIndex(
                name: "IX_intervenants_credits_niveau_responsabilite",
                table: "intervenants_credits",
                column: "niveau_responsabilite");

            migrationBuilder.CreateIndex(
                name: "IX_intervenants_credits_numero_contrat_credit",
                table: "intervenants_credits",
                column: "numero_contrat_credit");

            migrationBuilder.CreateIndex(
                name: "IX_intervenants_credits_numero_contrat_credit_date_declaration_id_excel",
                table: "intervenants_credits",
                columns: new[] { "numero_contrat_credit", "date_declaration", "id_excel" });

            migrationBuilder.CreateIndex(
                name: "IX_lieux_code_agence",
                table: "lieux",
                column: "code_agence");

            migrationBuilder.CreateIndex(
                name: "IX_lieux_code_pays",
                table: "lieux",
                column: "code_pays");

            migrationBuilder.CreateIndex(
                name: "IX_lieux_code_wilaya",
                table: "lieux",
                column: "code_wilaya");

            migrationBuilder.CreateIndex(
                name: "IX_lieux_id_lieu",
                table: "lieux",
                column: "id_lieu");

            migrationBuilder.CreateIndex(
                name: "IX_mapping_colonnes_id_mapping",
                table: "mapping_colonnes",
                column: "id_mapping");

            migrationBuilder.CreateIndex(
                name: "IX_monnaies_code",
                table: "monnaies",
                column: "code");

            migrationBuilder.CreateIndex(
                name: "IX_niveaux_responsabilite_code",
                table: "niveaux_responsabilite",
                column: "code");

            migrationBuilder.CreateIndex(
                name: "IX_parametrage_fichiers_xml_id",
                table: "parametrage_fichiers_xml",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_pays_code",
                table: "pays",
                column: "code");

            migrationBuilder.CreateIndex(
                name: "IX_pistes_audit_id_action",
                table: "pistes_audit",
                column: "id_action");

            migrationBuilder.CreateIndex(
                name: "IX_pistes_audit_matricule_utilisateur",
                table: "pistes_audit",
                column: "matricule_utilisateur");

            migrationBuilder.CreateIndex(
                name: "IX_regles_validation_nom_colonne",
                table: "regles_validation",
                column: "nom_colonne");

            migrationBuilder.CreateIndex(
                name: "IX_situations_credit_code",
                table: "situations_credit",
                column: "code");

            migrationBuilder.CreateIndex(
                name: "IX_table_intermediaire_traitement_date_declaration",
                table: "table_intermediaire_traitement",
                column: "date_declaration");

            migrationBuilder.CreateIndex(
                name: "IX_table_intermediaire_traitement_id_import_excel",
                table: "table_intermediaire_traitement",
                column: "id_import_excel");

            migrationBuilder.CreateIndex(
                name: "IX_table_intermediaire_traitement_numero_contrat",
                table: "table_intermediaire_traitement",
                column: "numero_contrat");

            migrationBuilder.CreateIndex(
                name: "IX_types_credit_code",
                table: "types_credit",
                column: "code");

            migrationBuilder.CreateIndex(
                name: "IX_types_garantie_code",
                table: "types_garantie",
                column: "code");

            migrationBuilder.CreateIndex(
                name: "IX_utilisateurs_matricule",
                table: "utilisateurs",
                column: "matricule",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_wilayas_code",
                table: "wilayas",
                column: "code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "erreurs_fichiers_excel");

            migrationBuilder.DropTable(
                name: "fichiers_xml");

            migrationBuilder.DropTable(
                name: "garanties");

            migrationBuilder.DropTable(
                name: "intervenants_credits");

            migrationBuilder.DropTable(
                name: "mapping_colonnes");

            migrationBuilder.DropTable(
                name: "parametrage_fichiers_xml");

            migrationBuilder.DropTable(
                name: "pistes_audit");

            migrationBuilder.DropTable(
                name: "table_intermediaire_traitement");

            migrationBuilder.DropTable(
                name: "tableau_de_bord");

            migrationBuilder.DropTable(
                name: "regles_validation");

            migrationBuilder.DropTable(
                name: "types_garantie");

            migrationBuilder.DropTable(
                name: "credits");

            migrationBuilder.DropTable(
                name: "intervenants");

            migrationBuilder.DropTable(
                name: "niveaux_responsabilite");

            migrationBuilder.DropTable(
                name: "activites_credit");

            migrationBuilder.DropTable(
                name: "classes_retard");

            migrationBuilder.DropTable(
                name: "durees_credit");

            migrationBuilder.DropTable(
                name: "fichiers_excel");

            migrationBuilder.DropTable(
                name: "lieux");

            migrationBuilder.DropTable(
                name: "monnaies");

            migrationBuilder.DropTable(
                name: "situations_credit");

            migrationBuilder.DropTable(
                name: "types_credit");

            migrationBuilder.DropTable(
                name: "utilisateurs");

            migrationBuilder.DropTable(
                name: "agences");

            migrationBuilder.DropTable(
                name: "pays");

            migrationBuilder.DropTable(
                name: "wilayas");
        }
    }
}
