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
('plafond_accorde', NULL, 'credits', 9, NULL, NULL, 0, 'bool'),
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
('type_garantie', 'type_garantie', 'garanties', 35, 'types_garantie', 'code', 0, 'string'),
('montant_garantie', 'montant_garantie', 'garanties', 36, NULL, NULL, 0, 'decimal'),
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
('018','Créance irrecouvrable'),
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


-- #region regles_validation 
INSERT INTO regles_validation (
    nom_colonne, type_regle, valeur_regle, message_erreur,
    colonne_dependante, valeur_dependante, colonne_cible, valeur_cible_attendue
) VALUES

('numero_contrat', 'OBLIGATOIRE', NULL, 'Le numéro de contrat est obligatoire.', NULL, NULL, NULL, NULL),
('situation_credit', 'OBLIGATOIRE', NULL, 'La situation du crédit est obligatoire.', NULL, NULL, NULL, NULL),
('role_niveau_responsabilite', 'OBLIGATOIRE', NULL, 'Le niveau de responsabilité est obligatoire.', NULL, NULL, NULL, NULL),
('code_agence', 'OBLIGATOIRE', NULL, 'Le code agence est obligatoire.', NULL, NULL, NULL, NULL),
('monnaie', 'OBLIGATOIRE', NULL, 'La monnaie est obligatoire.', NULL, NULL, NULL, NULL) ,
('type_credit', 'OBLIGATOIRE', NULL, 'Le type de crédit est obligatoire.', NULL, NULL, NULL, NULL),
('duree_initiale', 'OBLIGATOIRE', NULL, 'La durée initiale est obligatoire.', NULL, NULL, NULL, NULL),
('duree_restante', 'OBLIGATOIRE', NULL, 'La durée restante est obligatoire.', NULL, NULL, NULL, NULL),
('credit_accorde', 'OBLIGATOIRE', NULL, 'Le crédit accordé est obligatoire.', NULL, NULL, NULL, NULL),
('solde_restant', 'OBLIGATOIRE', NULL, 'Le solde restant est obligatoire.', NULL, NULL, NULL, NULL),
('montant_interets_courus', 'OBLIGATOIRE', NULL, 'Le montant des intérêts courus est obligatoire.', NULL, NULL, NULL, NULL),

('date_declaration', 'TYPE_DATE', 'dateonly', 'Le champ date_declaration doit être une date.', NULL, NULL, NULL, NULL),
('credit_accorde', 'TYPE_DECIMAL', 'decimal', 'Le champ credit_accorde doit être un nombre décimal.', NULL, NULL, NULL, NULL),
('solde_restant', 'TYPE_DECIMAL', 'decimal', 'Le champ solde_restant doit être un nombre décimal.', NULL, NULL, NULL, NULL),
('cout_total_credit', 'TYPE_DECIMAL', 'decimal', 'Le champ cout_total_credit doit être un nombre décimal.', NULL, NULL, NULL, NULL),
('mensualite', 'TYPE_DECIMAL', 'decimal', 'Le champ mensualite doit être un nombre décimal.', NULL, NULL, NULL, NULL),
('taux', 'TYPE_DECIMAL', 'decimal', 'Le champ taux doit être un nombre décimal.', NULL, NULL, NULL, NULL),
('date_constatation', 'TYPE_DATE', 'dateonly', 'Le champ date_constatation doit être une date.', NULL, NULL, NULL, NULL),
('nombre_echeances_impayes', 'TYPE_ENTIER', 'int', 'Le champ nombre_echeances_impayes doit être un entier.', NULL, NULL, NULL, NULL),
('montant_interets_courus', 'TYPE_DECIMAL', 'decimal', 'Le champ montant_interets_courus doit être un nombre décimal.', NULL, NULL, NULL, NULL),
('montant_capital_retard', 'TYPE_DECIMAL', 'decimal', 'Le champ montant_capital_retard doit être un nombre décimal.', NULL, NULL, NULL, NULL),
('montant_interets_retard', 'TYPE_DECIMAL', 'decimal', 'Le champ montant_interets_retard doit être un nombre décimal.', NULL, NULL, NULL, NULL),
('date_rejet', 'TYPE_DATE', 'dateonly', 'Le champ date_rejet doit être une date.', NULL, NULL, NULL, NULL),
('date_octroi', 'TYPE_DATE', 'dateonly', 'Le champ date_octroi doit être une date.', NULL, NULL, NULL, NULL),
('date_expiration', 'TYPE_DATE', 'dateonly', 'Le champ date_expiration doit être une date.', NULL, NULL, NULL, NULL),
('montant_garantie', 'TYPE_DECIMAL', 'decimal', 'Le champ montant_garantie doit être un nombre décimal.', NULL, NULL, NULL, NULL),

('role_niveau_responsabilite', 'DOMAINE', 'niveaux_responsabilite', 'La valeur de niveau de responsabilité est hors domaine autorisé.', NULL, NULL, NULL, NULL),

('id_plafond', 'OBLIGATOIRE_SI', NULL, 'Le champ numero_plafond est obligatoire si type_credit est 900.', 'type_credit', '900', NULL, NULL), -- marche

('num_contrat_credit', 'FORMAT', 'CR+[code_banque]+[date.HHMMSS]', 'Le format du numéro de contrat de crédit est invalide.', NULL, NULL, NULL, NULL),

('monnaie', 'DOMAINE', 'monnaies', 'La valeur de code monnaie est hors domaine autorisé.', NULL, NULL, NULL, NULL),

('participant_rib', 'LONGUEUR', '20', 'Le RIB a une taille differente de 20 positions.', NULL, NULL, NULL, NULL),

('code_pays', 'DOMAINE', 'pays', 'La valeur de code pays est hors domaine autorisé.', NULL, NULL, NULL, NULL),

-- faut ajouter codes agences   

('code_wilaya', 'OBLIGATOIRE_SI', NULL, 'Le code wilaya est obligatoire si le pays est l’Algérie.', 'code_pays', '012', NULL, NULL), --marche
('code_wilaya', 'DOMAINE', 'wilayas', 'La valeur de code wilaya est hors domaine autorisé.', NULL, NULL, NULL, NULL),
('code_wilaya', 'DOIT_ETRE_NULL_SI', NULL, 'Le code wilaya ne doit pas être renseigné si le pays n’est pas l’Algérie.', 'code_pays', '!=012', NULL, NULL),

('code_activite', 'DOMAINE', 'activites_credit', 'La valeur de code activité est hors domaine autorisé.', NULL, NULL, NULL, NULL),

('type_credit', 'DOMAINE', 'types_credit', 'La valeur de type crédit est hors domaine autorisé.', NULL, NULL, NULL, NULL),
('type_credit', 'VALEURS_INTERDITES_SI', '050,051,052,080', 'Type crédit interdit pour débiteur du type : entreprise (i3).', 'participant_type_cle', 'i3', NULL, NULL),
        
('situation_credit', 'DOMAINE', 'situations_credit', 'La valeur de situation crédit est hors domaine autorisé.', NULL, NULL, NULL, NULL),
('situation_credit', 'EGAL_A_SI', '900', 'La situation crédit doit être 900 si type_credit est 900.', 'type_credit', '900', 'situation_credit', '900'), 
    
('classe_retard', 'DOMAINE', 'classes_retard', 'La valeur de classe retard est hors domaine autorisé.', NULL, NULL, NULL, NULL), --marche
('classe_retard', 'OBLIGATOIRE_SI', NULL, 'Classe retard doit être renseignée si situation_credit est dans [010,011,012,013,015].', 'situation_credit', '010,011,012,013,015', NULL, NULL), --marche
('classe_retard', 'DOIT_ETRE_NULL_SI', NULL, 'Classe retard ne doit pas être renseignée si situation_credit est dans [001,002,020,900,014,018,005].', 'situation_credit', '001,002,020,900,014,018,005', NULL, NULL), --marche
('classe_retard', 'DOIT_ETRE_NULL_SI', NULL, 'Classe retard ne doit pas être renseignée si id_plafond est 900.', 'id_plafond', '900', NULL, NULL),

('duree_initiale', 'DOMAINE', 'durees_credit', 'La valeur de durée initiale est hors domaine autorisé.', NULL, NULL, NULL, NULL),
('duree_initiale', 'VALEUR_INTERDITE', '000', 'La durée initiale ne doit pas être 000.', NULL, NULL, NULL, NULL),
('duree_initiale', 'EGAL_A_SI', '999', 'La durée initiale doit être 999 si situation_credit est 001.', 'situation_credit', '001', 'duree_initiale', '999'),
('duree_initiale', 'VALEURS_INTERDITES_SI_PAS', '999', 'La durée initiale ne doit pas être 999 si situation_credit n’est pas 001.', 'situation_credit', '001', NULL, NULL),
('duree_initiale', 'EGAL_A_SI', '900', 'La durée initiale doit être 900 si type_credit est 900.', 'type_credit', '900', 'duree_initiale', '900'),
('duree_initiale', 'VALEURS_INTERDITES_SI_PAS', '900', 'La durée initiale ne doit pas être 900 si type_credit n’est pas 900.', 'type_credit', '900', NULL, NULL),

('duree_restante', 'DOMAINE', 'durees_credit', 'La valeur de durée restante est hors domaine autorisé.', NULL, NULL, NULL, NULL),
('duree_restante', 'OBLIGATOIRE_SI', NULL, 'La durée restante doit être renseignée si credit_accorde est renseigné.', 'credit_accorde', 'NOT_NULL', NULL, NULL),
('duree_restante', 'EGAL_A_SI', '999', 'La durée restante doit être 999 si situation_credit est 001.', 'situation_credit', '001', 'duree_rest', '999'),
('duree_restante', 'EGAL_A_SI', '900', 'La durée restante doit être 900 si type_credit est 900.', 'type_credit', '900', 'duree_rest', '900'),

('credit_accorde', 'EGAL_A_SI', '0', 'Le crédit accordé doit être 0 si type_credit est 900.', 'type_credit', '900', 'credit_accorde', '0'),
('credit_accorde', 'EGAL_A_SI', '0', 'Le crédit accordé doit être 0 si type_credit est 001.', 'type_credit', '001', 'credit_accorde', '0'),
('credit_accorde', 'SUP_A_SI', '0', 'Le crédit accordé doit être > 0 pour les autres types de crédit.', 'type_credit', 'AUTRES', NULL, NULL),

('solde_restant', 'OBLIGATOIRE', NULL, 'Le solde restant est obligatoire.', NULL, NULL, NULL, NULL),
('solde_restant', 'EGAL_A_SI', '0', 'Le solde restant doit être 0 si type_credit n’est pas 900 et situation_credit est 900.', 'type_credit,situation_credit', '!=900,900', 'solde_restant', '0'),
('solde_restant', 'EGAL_A_SI', '0', 'Le solde restant doit être 0 si situation_credit est egal à l''un de ces 4 : [001,020,005,018].', 'situation_credit', '001,020,018', 'solde_restant', '0'),

('cout_total_credit', 'OBLIGATOIRE_SI', NULL, 'Le coût total est obligatoire pour type_credit dans [050,051,052] et débiteur i1 ou i2.', 'type_credit', '050,051,052', 'participant_type_cle', 'i1,i2'),
('cout_total_credit', 'DOIT_ETRE_NULL_OU_ZERO_SI', NULL, 'Le coût total ne doit pas être renseigné ou doit être 0 si type_credit dans [050,051,052] et situation_credit dans [001,005,020,018].', 'type_credit', '050,051,052', 'situation_credit', '001,005,020,018'),
('mensualite', 'OBLIGATOIRE_SI', NULL, 'La mensualité est obligatoire pour type_credit dans [050,051,052].', 'type_credit', '050,051,052', NULL, NULL),
('mensualite', 'DOIT_ETRE_NULL_SI', NULL, 'La mensualité ne doit pas être renseignée si type_credit n’est pas dans [050,051,052].', 'type_credit', '!=050,051,052', NULL, NULL),
('mensualite', 'DOIT_ETRE_NULL_OU_ZERO_SI', NULL, 'La mensualité ne doit pas être renseignée ou doit être 0 si type_credit dans [050,051,052] et situation_credit dans [001,005,020,018].', 'type_credit', '050,051,052', 'situation_credit', '001,005,020,018'),
('montant_garantie', 'SUP', '0', 'Le montant de la garantie doit être supérieur à 0.', NULL, NULL, NULL, NULL),
('montant_garantie', 'EGAL_A_SI', '0', 'Le montant de la garantie doit être 0 si type_garantie est 999.', 'type_garantie', '999', 'montant_garantie', '0');


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