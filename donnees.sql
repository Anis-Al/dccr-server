-- #region mapping excel-bdd
INSERT INTO mapping_colonnes (colonne_excel, colonne_bdd, table_bdd, ordre, nom_table_lookup, colonne_cle_lookup, obligatoire, type_donnee) VALUES
('cli', 'participant_cli', 'intervenants', 1, NULL, NULL, 0, 'string'),
('date_declaration', 'date_declaration', 'credits', 2, NULL, NULL, 1, 'dateonly'),
('type_cle', 'participant_type_cle', 'intervenants', 3, NULL, NULL, 0, 'string'),
('cle', 'participant_cle', 'intervenants', 4, NULL, NULL, 1, 'string'),
('nifd', 'participant_nif', 'intervenants', 5, NULL, NULL, 0, 'string'),
('cle_int', NULL, NULL, 6, NULL, NULL, 0, 'string'),
('cle_onomastique', NULL, NULL, 7, NULL, NULL, 0, 'string'),
('niveau_respons', 'role_niveau_responsabilite', 'intervenants_credits', 8, 'niveaux_responsabilite', 'code', 0, 'string'),
('plafond_accorde', Null, 'credits', 9, NULL, NULL, 0, 'bool'), -- a ajouter apres 
('numero_contrat', 'numero_contrat', 'credits', 10, NULL, NULL, 1, 'string'),
('dev', 'monnaie', 'credits', 11, 'monnaies', 'code', 1, 'string'),
('rib', 'participant_rib', 'intervenants', 12, NULL, NULL, 0, 'string'),
('cpay', 'code_pays', 'lieux', 13, 'pays', 'code', 1, 'string'),
('age', 'code_agence', 'lieux', 14, 'agences', 'code', 1, 'string'),
('code_wilaya', 'code_wilaya', 'lieux', 15, 'wilayas', 'code', 1, 'string'),
('code_act_credit', 'activite_credit', 'credits', 16, 'activites_credit', 'code', 1, 'string'),
('type_credit', 'type_credit', 'credits', 17, 'types_credit', 'code', 1, 'string'),
('situation_credit', 'situation_credit', 'credits', 18, 'situations_credit', 'code', 1, 'string'),
('classe_retard', 'classe_retard', 'credits', 19, 'classes_retard', 'code', 0, 'string'),
('duree_init', 'duree_initiale', 'credits', 20, 'durees_credit', 'code', 0, 'string'),
('duree_restante', 'duree_restante', 'credits', 21, 'durees_credit', 'code', 0, 'string'),
('credit_accorde', 'credit_accorde', 'credits', 22, NULL, NULL, 1, 'decimal'),
('solde_restant', 'solde_restant', 'credits', 23, NULL, NULL, 0, 'decimal'),
('cout_credit', 'cout_total_credit', 'credits', 24, NULL, NULL, 0, 'decimal'),
('mensualite', 'mensualite', 'credits', 25, NULL, NULL, 0, 'decimal'),
('taux', 'taux', 'credits', 26, NULL, NULL, 0, 'decimal'),
('date_impaye', 'date_constatation', 'credits', 27, NULL, NULL, 0, 'date'),
('nombre_impaye', 'nombre_echeances_impayes', 'credits', 28, NULL, NULL, 0, 'int'),
('montant_nom_rmb', 'montant_interets_courus', 'credits', 29, NULL, NULL, 0, 'decimal'),
('montant_cap_retard', 'montant_capital_retard', 'credits', 30, NULL, NULL, 0, 'decimal'),
('montant_int_retard', 'montant_interets_retard', 'credits', 31, NULL, NULL, 0, 'decimal'),
('date_rejet', 'date_rejet', 'credits', 32, NULL, NULL, 0, 'date'),
('date_octroi', 'date_octroi', 'credits', 33, NULL, NULL, 0, 'date'),
('date_expi', 'date_expiration', 'credits', 34, NULL, NULL, 0, 'date'),
('type_garantie', 'garantie_type_garantie', 'garanties', 35, 'types_garantie', 'code', 0, 'string'),
('montant_garantie', 'garantie_montant_garantie', 'garanties', 36, NULL, NULL, 0, 'decimal'),
('type_credit_declare', NULL, NULL, 37, NULL, NULL, 0, 'string'),
('numero_plafond', 'id_plafond', 'credits', 38, NULL, NULL, 0, 'string'),
('statut', NULL, NULL, 39, NULL, NULL, 0, 'string'),
('motif', 'motif', 'credits', 40, NULL, NULL, 0, 'string'),
('date_exe', 'date_execution', NULL, 41, NULL, NULL, 0, 'date');
-- #endregion

-- #region tables des domaines
INSERT INTO durees_credit(code,domaine) VALUES
('000', '0 jour'),
('001', 'Jusqu''à 90 jours'),
('002', 'Plus de 90 jours jusqu''à 180 jours'),
('003', 'Plus de 180 jours à 1 an'),
('010', 'Plus de 1 an à 5 ans'),
('011', 'Plus de 5 an à 7 ans'),
('005', 'Plus de 7 jusqu''à 10 ans'),
('006', 'Plus de 10 jusqu''à 20 ans'),
('007', 'Plus de 20 à 25 ans'),
('008', 'Plus de 25 à 30 ans'),
('009', 'Plus de 30 ans'),
('900', 'Sans durée spécifique (plafond)'),
('999', 'Durée indéterminée');

INSERT INTO monnaies (code, domaine) VALUES
('DZD', 'Dinar Algérien'),
('DVS', 'Monnaie étrangère');

INSERT INTO types_credit (code, domaine) VALUES
('010', 'Comptes ordinaires débiteurs'),
('020', 'Crédits à l’exportation'),
('030', 'Créances commerciales sur l’Algérie'),
('040', 'Crédits de trésorerie'),
('050', 'Crédits à la consommation'),
('051', 'Crédits Véhicule'),
('052', 'Crédits au personnel (BA et BEF)'),
('053', 'Carte de crédit'),
('060', 'Crédits d’investissement'),
('070', 'Crédits aidés'),
('080', 'Crédits immobiliers à l’Habitat'),
('081', 'Crédits immobiliers aux promoteurs'),
('090', 'Crédit-bail mobilier'),
('091', 'Crédit-bail immobilier'),
('100', 'Crédits restructurés'),
('110', 'Engagements des garanties'),
('120', 'Engagements de financement'),
('900', 'Plafond des crédits accordés');


INSERT INTO activites_credit (code, domaine) VALUES
('001', 'Agriculture, Chasse, Services Annexes'),
('002', 'Sylvicultures, Exploitation Forestière, Services Annexes'),
('005', 'Pêche, Aquaculture'),
('010', 'Extraction de Houille, de Lignite et de Tourbe'),
('011', 'Extraction d''Hydrocarbures ; Services Annexes'),
('012', 'Extraction de Minerais d''Uranium'),
('013', 'Extraction de Minerais Métalliques'),
('014', 'Autres Industries Extractives'),
('015', 'Industries Alimentaires'),
('016', 'Industrie du Tabac'),
('017', 'Industrie Textile'),
('018', 'Industrie de l''Habillement et des Fourrures'),
('019', 'Industrie du Cuir et de la Chaussure'),
('020', 'Travail du Bois et Fabrication d''Articles en Bois'),
('021', 'Industrie du Papier et du Carton'),
('022', 'Édition, Imprimerie, Reproduction'),
('023', 'Cokéfaction, Raffinage, Industries Nucléaires'),
('024', 'Industrie Chimique'),
('025', 'Industrie du Caoutchouc et des Plastiques'),
('026', 'Fabrication d''autres Produits Minéraux non Métallique'),
('027', 'Métallurgie'),
('028', 'Travail des Métaux'),
('029', 'Fabrication de Machines et Équipements'),
('030', 'Fabrication de Machines de Bureau et de Matériel Informatique'),
('031', 'Fabrication de Machines et Appareils Électriques'),
('032', 'Fabrication d''Équipements de Radio, Télévision et Communication'),
('033', 'Fabrication d''Instruments Médicaux, Précision, Optique, d''Horlogerie'),
('034', 'Industrie Automobile'),
('035', 'Fabrication d''Autres Matériels de Transport'),
('036', 'Fabrication de Meubles ; Industries Diverses'),
('037', 'Récupération'),
('040', 'Production et Distribution d''Électricité, de Gaz et de Chaleur'),
('041', 'Captage, Traitement et Distribution d''Eau'),
('045', 'Construction'),
('050', 'Commerce et Réparation Automobile'),
('051', 'Commerce de Gros et Intermédiaires du Commerce'),
('052', 'Commerce de détail et réparation d''articles domestiques'),
('055', 'Hôtels et Restaurants'),
('060', 'Transports Terrestres'),
('061', 'Transports Par Eau'),
('062', 'Transports Aériens'),
('063', 'Services Auxiliaires des Transports'),
('064', 'Postes et Télécommunications'),
('065', 'Intermédiation Financière'),
('066', 'Assurance'),
('067', 'Auxiliaires Financiers et d''Assurance'),
('070', 'Activités Immobilières'),
('071', 'Location sans Opérateur'),
('072', 'Activités Informatiques'),
('073', 'Recherche et Développement'),
('074', 'Services Fournis Principalement aux Entreprises'),
('075', 'Administration Publique'),
('080', 'Éducation'),
('085', 'Santé et Action Sociale'),
('090', 'Assainissement, Voirie et Gestion des Déchets'),
('091', 'Activités Associatives'),
('092', 'Activités Récréatives, Culturelles et Sportives'),
('093', 'Services Personnels'),
('095', 'Services Domestiques'),
('099', 'Activités Extraterritoriales'),
('000', 'Sans information');

INSERT INTO classes_retard (code, domaine) VALUES
('001', 'Jusqu''à 30 jours'),
('002', 'Plus de 30 à 60 jours'),
('003', 'Plus de 60 à 90 jours'),
('010', 'Plus de 90 à 180 jours'),
('020', 'Plus de 180 à 270 jours'),
('021', 'Plus de 270 à 360 jours'),
('030', 'Plus de 12 à 15 mois'),
('031', 'Plus de 15 à 18 mois'),
('032', 'Plus de 18 à 24 mois'),
('033', 'Plus de 24 à 30 mois'),
('034', 'Plus de 30 à 36 mois'),
('035', 'Plus de 36 à 48 mois'),
('036', 'Plus de 48 à 60 mois'),
('037', 'Plus de 60 mois');

INSERT INTO niveaux_responsabilite (code, domaine) VALUES
('001', 'Emprunteur'),
('002', 'Emprunteur Principal'),
('003', 'Co-emprunteur'),
('004', 'Crédit - preneur'),
('005', 'Garant');

INSERT INTO situations_credit (code, domaine) VALUES
('001', 'Crédit rejeté'),
('900', 'Crédit à risque potentiel'),
('002', 'Crédit régulier'),
('010', 'Crédit impayé'),
('005','Crédit annulé'),
('011', 'Créance à problèmes potentiels'),
('012', 'Créance très risquée'),
('013', 'Créance compromise'),
('014', 'Créance classée restructurée'),
('015', 'Défaillance du débiteur'),
('020', 'Crédit remboursé'),
('030','Créance abandonnée'),
('040','Créance cédée'),
('050','Mise en jeu de la garantie');

INSERT INTO types_garantie (code, domaine) VALUES
('010', 'Hypothèques'),
('011', 'Cautions hypothécaires'),
('020', 'Nantissements (fonds de commerce, équipements et matériels roulants ….)'),
('021', 'Nantissements sur véhicules automobiles (Gages sur véhicules)'),
('022', 'Nantissements de titres et autres valeurs mobilières'),
('030', 'Dépôts de fonds (provisions sur Credoc, provision sur cautions …)'),
('090', 'Autres suretés réelles'),
('100', 'Garanties de l’État'),
('101', 'Cautions conjointes, solidaires et indivisibles'),
('102', 'Cautions de tiers (personnes physiques)'),
('103', 'Cautions de tiers (personnes morales)'),
('110', 'Avals'),
('120', 'Garanties des organismes publics'),
('121', 'Garanties des sociétés d’assurances crédit'),
('999','Garanties non exigées'),
('777','Garanties non recueillies'),
('140','Garantie des bangues et établissements financiers'),
('190','Autres suretés personnelles');


INSERT INTO wilayas (code, domaine) VALUES
('01', 'ADRAR'),
('02', 'CHLEF'),
('03', 'LAGHOUAT'),
('04', 'OUM ELBOUAGHI'),
('05', 'BATNA'),
('06', 'BEJAIA'),
('07', 'BISKRA'),
('08', 'BECHAR'),
('09', 'BLIDA'),
('10', 'BOUIRA'),
('11', 'TAMANGHASSET'),
('12', 'TEBESSA'),
('13', 'TLEMCEN'),
('14', 'TIARET'),
('15', 'TIZI.OUZOU'),
('16', 'ALGER'),
('17', 'DJELFA'),
('18', 'JIJEL'),
('19', 'SETIF'),
('20', 'SAIDA'),
('21', 'SKIKDA'),
('22', 'SIDI BEL ABBES'),
('23', 'ANNABA'),
('24', 'GUELMA'),
('25', 'CONSTANTINE'),
('26', 'MEDEA'),
('27', 'MOSTAGANEM'),
('28', 'M''SILA'),
('29', 'MASCARA'),
('30', 'OUARGLA'),
('31', 'ORAN'),
('32', 'EL BAYADH'),
('33', 'ILLIZI'),
('34', 'B.B.ARREIDJ'),
('35', 'BOUMERDES'),
('36', 'AL TARF'),
('37', 'TINDOUF'),
('38', 'TISSEMSILT'),
('39', 'EL OUED'),
('40', 'KHENCHELA'),
('41', 'SOUKAHRAS'),
('42', 'TIPAZA'),
('43', 'MILA'),
('44', 'AIN DEFLA'),
('45', 'NAAMA'),
('46', 'AIN TEMOUCHENT'),
('47', 'GHARDAIA'),
('48', 'RELIZANE');

INSERT INTO pays (code, domaine) VALUES
('004', 'Afghanistan'),
('710', 'Afrique du Sud'),
('248', 'Aland (îles)'),
('008', 'Albanie'),
('012', 'Algérie'),
('276', 'Allemagne'),
('020', 'Andorre'),
('024', 'Angola'),
('660', 'Anguilla'),
('010', 'Antarctique'),
('028', 'Antigua-et-Barbuda'),
('682', 'Arabie saoudite'),
('032', 'Argentine'),
('051', 'Arménie'),
('533', 'Aruba'),
('036', 'Australie'),
('040', 'Autriche'),
('031', 'Azerbaïdjan'),
('044', 'Bahamas'),
('048', 'Bahreïn'),
('050', 'Bangladesh'),
('052', 'Barbade'),
('250', 'France'),
('840', 'États-Unis');
-- #endregion


-- #region regles_validation (CLEAN STRUCTURE - NO nom_table)
-- === Required Fields ===
INSERT INTO regles_validation (
    nom_colonne, type_regle, valeur_regle, message_erreur, categorie_regle,
    colonne_dependante, valeur_dependante, colonne_cible, valeur_cible_attendue
) VALUES
('numero_contrat', 'REQUIRED', NULL, 'Le numéro de contrat est obligatoire.', 'REQUIRED', NULL, NULL, NULL, NULL),
('situation_credit', 'REQUIRED', NULL, 'La situation du crédit est obligatoire.', 'REQUIRED', NULL, NULL, NULL, NULL);

-- === Type Checks ===
INSERT INTO regles_validation (
    nom_colonne, type_regle, valeur_regle, message_erreur, categorie_regle,
    colonne_dependante, valeur_dependante, colonne_cible, valeur_cible_attendue
) VALUES
('montant_credit', 'TYPE', 'decimal', 'Le montant doit être un nombre décimal.', 'TYPE', NULL, NULL, NULL, NULL);

-- === Lookup Checks ===
INSERT INTO regles_validation (
    nom_colonne, type_regle, valeur_regle, message_erreur, categorie_regle,
    colonne_dependante, valeur_dependante, colonne_cible, valeur_cible_attendue
) VALUES
('classe_retard', 'DOMAINES', 'classes_retard', 'La classe de retard est invalide.', 'SIMPLE', NULL, NULL, NULL, NULL),
('monnaie', 'DOMAINES', 'monnaies', 'La monnaie est invalide.', 'SIMPLE', NULL, NULL, NULL, NULL),
('type_garantie', 'DOMAINES', 'types_garantie', 'Le type de garantie est invalide.', 'SIMPLE', NULL, NULL, NULL, NULL),
('type_credit', 'DOMAINES', 'types_credit', 'Le type de crédit est invalide.', 'SIMPLE', NULL, NULL, NULL, NULL),
('situation_credit', 'DOMAINES', 'situations_credit', 'La situation du crédit est invalide.', 'SIMPLE', NULL, NULL, NULL, NULL),
('activite_credit', 'DOMAINES', 'activites_credit', 'L''activité crédit est invalide.', 'SIMPLE', NULL, NULL, NULL, NULL),
('code_wilaya', 'DOMAINES', 'wilayas', 'Le code wilaya n''existe pas.', 'SIMPLE', NULL, NULL, NULL, NULL);

-- === Dependency: type_garantie/montant_garantie must be null if not guarantor ===
INSERT INTO regles_validation (
    nom_colonne, type_regle, valeur_regle, message_erreur, categorie_regle,
    colonne_dependante, valeur_dependante, colonne_cible, valeur_cible_attendue
) VALUES
('type_garantie', 'NULL_IF_NOT', NULL, 'Type de garantie doit être vide si le participant n\''est pas garant.', 'DEPENDENCY', 'niveau_respons', '005', NULL, NULL),
('montant_garantie', 'NULL_IF_NOT', NULL, 'Montant de garantie doit être vide si le participant n\''est pas garant.', 'DEPENDENCY', 'niveau_respons', '005', NULL, NULL);

-- === Dependency (Conditional Required) ===
INSERT INTO regles_validation (
    nom_colonne, type_regle, valeur_regle, message_erreur, categorie_regle,
    colonne_dependante, valeur_dependante, colonne_cible, valeur_cible_attendue
) VALUES
('montant_garantie', 'REQUIRED_IF', NULL, 'Le montant de garantie est obligatoire pour les garants.', 'GUARANTOR', 'role_niveau_responsabilite', 5, NULL, NULL),
('code_wilaya', 'REQUIRED_IF', NULL, 'La wilaya est obligatoire si le pays est l''Algérie.', 'DEPENDENCY', 'code_pays', 12, NULL, NULL);

-- === Consistency (Coherence) ===
INSERT INTO regles_validation (
    nom_colonne, type_regle, valeur_regle, message_erreur, categorie_regle,
    colonne_dependante, valeur_dependante, colonne_cible, valeur_cible_attendue
) VALUES
('montant_credit', 'CONSISTENT_ACROSS_GROUP', NULL, 'Montant du crédit incohérent entre les lignes du même prêt.', 'CONSISTENCY', NULL, NULL, NULL, NULL),
('code_wilaya', 'CONSISTENT_ACROSS_GROUP', NULL, 'Incohérence détectée entre les lignes du même crédit (code_wilaya).', 'CONSISTENCY', NULL, NULL, NULL, NULL);

-- === Equality/Dependency Rules ===
INSERT INTO regles_validation (
    nom_colonne, type_regle, colonne_dependante, valeur_dependante, colonne_cible, valeur_cible_attendue, categorie_regle, message_erreur
) VALUES
('duree_restante', 'EQUALS', 'type_credit', '900', 'duree_restante', '900', 'DEPENDENCY', 'Si type_credit = 900, duree_restante doit être 900.'),
('duree_restante', 'EQUALS', 'situation_credit', '1', 'duree_restante', '999', 'DEPENDENCY', 'Si situation_credit = 1, duree_restante doit être 999.'),
('solde_restant', 'EQUALS', 'type_credit', '1', 'solde_restant', '0', 'DEPENDENCY', 'Si type_credit = 1, solde_restant doit être 0.');

-- === Not Null (Conditional) ===
INSERT INTO regles_validation (
    nom_colonne, type_regle, colonne_dependante, valeur_dependante, colonne_cible, categorie_regle, message_erreur
) VALUES
('cout_total_credit', 'NOT_NULL', 'type_credit', '50', 'cout_total_credit', 'DEPENDENCY', 'Si type_credit = 50, cout_total_credit ne doit pas être nul.'),
('cout_total_credit', 'NOT_NULL', 'type_credit', '51', 'cout_total_credit', 'DEPENDENCY', 'Si type_credit = 51, cout_total_credit ne doit pas être nul.'),
('cout_total_credit', 'NOT_NULL', 'type_credit', '52', 'cout_total_credit', 'DEPENDENCY', 'Si type_credit = 52, cout_total_credit ne doit pas être nul.');

-- #endregion

INSERT INTO utilisateurs (
    matricule,
    nom_complet,
    mot_de_passe,
    role
) VALUES (
    'anis2002',  
    'Alim Anis', 
    0xb1f13e6f184769053d8201dcc5597cc1e6d1140933e60212985ca622888921e465a51468614c3982a695115c90343acc664ba8d6c52e99e43590742889b60e49, 
    1
);