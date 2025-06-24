-- #region mapping excel-bdd
INSERT INTO mapping_colonnes (colonne_excel, colonne_bdd, colonne_prod, table_prod, type_donnee_prod) VALUES
('cli', 'participant_cli', 'cli', 'intervenants', 'string'),
('date_declaration', 'date_declaration', 'date_declaration', 'credits', 'dateonly'),
('type_cle', 'participant_type_cle', 'type_cle', 'intervenants', 'string'),
('cle', 'participant_cle', 'cle', 'intervenants', 'string'),
('nifd', 'participant_nif', 'nif', 'intervenants', 'string'),
('cle_int', NULL, NULL, NULL, 'string'),
('cle_onomastique', NULL, NULL, NULL, 'string'),
('niveau_respons', 'role_niveau_responsabilite', 'niveau_responsabilite', 'intervenants_credits', 'string'),
('plafond_accorde', NULL, NULL, 'credits', 'bool'),
('numero_contrat', 'numero_contrat', 'numero_contrat_credit', 'credits', 'string'),
('dev', 'monnaie', 'monnaie', 'credits', 'string'),
('rib', 'participant_rib', 'rib', 'intervenants', 'string'),
('cpay', 'code_pays', 'code_pays', 'lieux', 'string'),
('age', 'code_agence', 'code_agence', 'lieux', 'string'),
('code_wilaya', 'code_wilaya', 'code_wilaya', 'lieux', 'string'),
('code_act_credit', 'activite_credit', 'activite_credit', 'credits', 'string'),
('type_credit', 'type_credit', 'type_credit', 'credits', 'string'),
('situation_credit', 'situation_credit', 'situation_credit', 'credits', 'string'),
('classe_retard', 'classe_retard', 'classe_retard', 'credits', 'string'),
('duree_init', 'duree_initiale', 'duree_initiale', 'credits', 'string'),
('duree_restante', 'duree_restante', 'duree_restante', 'credits', 'string'),
('credit_accorde', 'credit_accorde', 'credit_accorde', 'credits', 'decimal'),
('solde_restant', 'solde_restant', 'solde_restant', 'credits', 'decimal'),
('cout_credit', 'cout_total_credit', 'cout_total_credit', 'credits', 'decimal'),
('mensualite', 'mensualite', 'mensualite', 'credits', 'decimal'),
('taux', 'taux', 'taux', 'credits', 'decimal'),
('date_impaye', 'date_constatation', 'date_constatation', 'credits', 'dateonly'),
('nombre_impaye', 'nombre_echeances_impayes', 'nombre_echeances_impayes', 'credits', 'entier'),
('montant_nom_rmb', 'montant_interets_courus', 'montant_interets_courus', 'credits', 'decimal'),
('montant_cap_retard', 'montant_capital_retard', 'montant_capital_retard', 'credits', 'decimal'),
('montant_int_retard', 'montant_interets_retard', 'montant_interets_retard', 'credits', 'decimal'),
('date_rejet', 'date_rejet', 'date_rejet', 'credits', 'dateonly'),
('date_octroi', 'date_octroi', 'date_octroi', 'credits', 'dateonly'),
('date_expi', 'date_expiration', 'date_expiration', 'credits', 'dateonly'),
('type_garantie', 'type_garantie', 'type_garantie', 'garanties', 'string'),
('montant_garantie', 'montant_garantie', 'montant_garantie', 'garanties', 'decimal'),
('type_credit_declare', NULL, NULL, NULL, 'string'),
('numero_plafond', 'id_plafond', 'id_plafond', 'credits', 'string'),
('statut', NULL, NULL, NULL, 'string'),
('motif', 'motif', 'motif', 'credits', 'string'),
('date_exe', 'date_execution', 'date_execution', 'credits', 'dateonly');
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

INSERT INTO agences (code, domaine, wilaya_code) VALUES
('00001', 'A. Mutualisée SIDI YAHIA', '16'),
('00002', 'Amirouche', '16'),
('00003', 'Cheraga', '16'),
('00004', 'A. Mutualisée Rouiba', '16'),
('00005', 'Didouche Mourad', '16'),
('00006', 'Kouba', '16'),
('00007', 'Bouzereah', '16'),
('00008', 'Val d''Hydra', '16'),
('00009', 'Draria', '16'),
('00010', 'Dely Ibrahim', '16'),
('00011', 'Beraki', '16'),
('00012', 'BIRKHADEM', '16'),
('00013', 'TIXSRAINE', '16'),
('00014', 'Hussein Dey', '16'),
('00015', 'Bab El oued', '16'),
('00016', 'les Sources', '16'),
('00017', 'Belouizdad', '16'),
('00018', 'El Harrach Belle Vue', '16'),
('00019', 'Bordj El Kiffan', '16'),
('00020', 'El Biar Ali Khodja', '16'),
('00021', 'Baba Hassen', '16'),
('00022', 'AFAK GRAND ENTR', '16'),
('00023', 'A. Mutualisée Ouled Fayet', '16'),
('00025', 'Bab Ezzouar', '16'),
('00027', 'Ain Benian', '16'),
('00029', 'Ain Naadja', '16'),
('00030', 'BENI MESSOUS', '16'),
('00031', 'Boumerdes', '35'),
('00032', 'Telemly', '16'),
('00035', 'Reghaia', '35'),
('00036', 'Zeralda', '16'),
('00101', 'Oran - Soummam', '31'),
('00102', 'Oran - Les Lions', '31'),
('00103', 'Oran - Yaghmoracen', '31'),
('00104', 'Oran - Loubet', '31'),
('00105', 'Oran - Les castors', '31'),
('00106', 'A. Mutualisée Oran Point du Jour', '31'),
('00107', 'Oran - Ain El Turk', '31'),
('00108', 'Oran - Es Senia', '31'),
('00109', 'ARZEW', '31'),
('00110', 'Oran - Bir El Djir', '31'),
('00151', 'MOSTAGANEM', '27'),
('00170', 'RELIZANE', '48'),
('00175', 'MASCARA', '29'),
('00180', 'TIARET', '14'),
('00201', 'Blida - Ben Boulaid', '09'),
('00202', 'BLIDA ZONE INDUSTRIELLE', '09'),
('00203', 'Boufarik', '09'),
('00204', 'Blida - Kritli Mokhtar', '09'),
('00225', 'Bouira', '10'),
('00251', 'Kolea', '42'),
('00271', 'Tipaza', '42'),
('00261', 'Medea', '26'),
('00301', 'A. Mutualisée Annaba', '23'),
('00302', 'Annaba - 1er novembre', '23'),
('00303', 'Annaba - Les Peupliers', '23'),
('00325', 'Biskra', '07'),
('00351', 'Skikda', '21'),
('00360', 'Souk Ahras', '41'),
('00361', 'El Taref', '36'),
('00375', 'Ouargla (PI)', '30'),
('00376', 'A. Mutualisée Hassi Messaoud', '30'),
('00401', 'Tlemcen - El Kiffan', '13'),
('00402', 'Tlemcen - Les Dahlias', '13'),
('00451', 'CHLEF', '02'),
('00475', 'Jijel', '18'),
('00501', 'A. Mutualisée Béjaia - Sidi Ahmed', '06'),
('00502', 'Akbou - Nouvelle Ville', '06'),
('00503', 'Béjaia - Seghir', '06'),
('00504', 'Béjaia - liberté', '06'),
('00506', 'El Kseur', '06'),
('00551', 'Ghardaia', '47'),
('00601', 'A. Mutualisée Constantine - Sidi Mabrouk', '25'),
('00602', 'Constantine - Belle Vue', '25'),
('00603', 'Constantine - El Khroub', '25'),
('00604', 'Constantine - Ali Mendjli', '25'),
('00610', 'Oum El Bouaghi', '04'),
('00620', 'Khenchla (PI)', '40'),
('00630', 'M''Sila', '28'),
('00651', 'Batna', '05'),
('00675', 'Ain Temouchent', '46'),
('00680', 'Mila', '43'),
('00701', 'A. Mutualisée Sidi Belabbes', '22'),
('00702', 'Sidi Bel Abbes 2', '22'),
('00751', 'Bordj Bou Arreridj', '34'),
('00752', 'Bordj Bou Arreridj El Mokrani', '34'),
('00801', 'Antenne Reatail - BC Sétif', '19'),
('00802', 'Sétif - El Hidhab (PI)', '19'),
('00803', 'Sétif - El Fouara', '19'),
('00804', 'El Eulma', '19'),
('00851', 'A. Mutualisée Tizi-Ouzou', '15'),
('00852', 'Azzazga', '15'),
('00853', 'Tizi Ouzou', '15');


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
('participant_cle', 'OBLIGATOIRE', NULL, 'La clé de débiteur est obligatoire.', NULL, NULL, NULL, NULL),
('participant_type_cle', 'OBLIGATOIRE', NULL, 'Le type de clé de débiteur est obligatoire.', NULL, NULL, NULL, NULL),
('participant_cli', 'OBLIGATOIRE', NULL, 'L''identifiant du client SGA est obligatoire.', NULL, NULL, NULL, NULL),
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

('date_declaration', 'TYPE', 'dateonly', 'Le champ date_declaration doit être une date.', NULL, NULL, NULL, NULL),
('credit_accorde', 'TYPE', 'decimal', 'Le champ credit_accorde doit être un nombre décimal.', NULL, NULL, NULL, NULL),
('solde_restant', 'TYPE', 'decimal', 'Le champ solde_restant doit être un nombre décimal.', NULL, NULL, NULL, NULL),
('cout_total_credit', 'TYPE', 'decimal', 'Le champ cout_total_credit doit être un nombre décimal.', NULL, NULL, NULL, NULL),
('mensualite', 'TYPE', 'decimal', 'Le champ mensualite doit être un nombre décimal.', NULL, NULL, NULL, NULL),
('taux', 'TYPE', 'decimal', 'Le champ taux doit être un nombre décimal.', NULL, NULL, NULL, NULL),
('date_constatation', 'TYPE', 'dateonly', 'Le champ date_constatation doit être une date.', NULL, NULL, NULL, NULL),
('nombre_echeances_impayes', 'TYPE', 'entier', 'Le champ nombre_echeances_impayes doit être un entier.', NULL, NULL, NULL, NULL),
('montant_interets_courus', 'TYPE', 'decimal', 'Le champ montant_interets_courus doit être un nombre décimal.', NULL, NULL, NULL, NULL),
('montant_capital_retard', 'TYPE', 'decimal', 'Le champ montant_capital_retard doit être un nombre décimal.', NULL, NULL, NULL, NULL),
('montant_interets_retard', 'TYPE', 'decimal', 'Le champ montant_interets_retard doit être un nombre décimal.', NULL, NULL, NULL, NULL),
('date_rejet', 'TYPE', 'dateonly', 'Le champ date_rejet doit être une date.', NULL, NULL, NULL, NULL),
('date_octroi', 'TYPE', 'dateonly', 'Le champ date_octroi doit être une date.', NULL, NULL, NULL, NULL),
('date_expiration', 'TYPE', 'dateonly', 'Le champ date_expiration doit être une date.', NULL, NULL, NULL, NULL),
('montant_garantie', 'TYPE', 'decimal', 'Le champ montant_garantie doit être un nombre décimal.', NULL, NULL, NULL, NULL),

('id_plafond', 'OBLIGATOIRE_SI', NULL, 'Le champ numero_plafond est obligatoire si type_credit est 900.', 'type_credit', '900', NULL, NULL), -- marche
('code_wilaya', 'OBLIGATOIRE_SI', NULL, 'Le code wilaya est obligatoire si le pays est l’Algérie.', 'code_pays', '012', NULL, NULL), --marche
('code_agence', 'OBLIGATOIRE_SI', NULL, 'Le code ''agence est obligatoire si le pays est l’Algérie.', 'code_pays', '012', NULL, NULL), --marche
('classe_retard', 'OBLIGATOIRE_SI', NULL, 'Classe retard doit être renseignée si situation_credit est dans [010,011,012,013,015].', 'situation_credit', '010,011,012,013,015', NULL, NULL), --marche
('duree_restante', 'OBLIGATOIRE_SI', NULL, 'La durée restante doit être renseignée si credit_accorde est renseigné.', 'credit_accorde', 'NOT_NULL', NULL, NULL),
('mensualite', 'OBLIGATOIRE_SI', NULL, 'La mensualité est obligatoire pour type_credit dans [050,051,052].', 'type_credit', '050,051,052', NULL, NULL),
('cout_total_credit', 'OBLIGATOIRE_SI', NULL, 'Le coût total est obligatoire pour type_credit dans [050,051,052] et débiteur i1 ou i2.', 'type_credit|participant_type_cle', '050,051,052|i1,i2', NULL, NULL),

('num_contrat', 'FORMAT', 'CR+[code_banque]+[date.HHMMSS]', 'Le format du numéro de contrat de crédit est invalide.', NULL, NULL, NULL, NULL),

('participant_rib', 'LONGUEUR', '20', 'Le RIB a une taille differente de 20 positions.', NULL, NULL, NULL, NULL),
('participant_cli', 'LONGUEUR', '15', 'L''identifiant du client SGA a une taille differente de 15 positions.', NULL, NULL, NULL, NULL),

('role_niveau_responsabilite', 'DOMAINE', 'niveaux_responsabilite', 'La valeur de niveau de responsabilité est hors domaine autorisé.', NULL, NULL, NULL, NULL),
('monnaie', 'DOMAINE', 'monnaies', 'La valeur de code monnaie est hors domaine autorisé.', NULL, NULL, NULL, NULL),
('code_pays', 'DOMAINE', 'pays', 'La valeur de code pays est hors domaine autorisé.', NULL, NULL, NULL, NULL),
('code_agence', 'DOMAINE', 'agences', 'La valeur de code agence est hors domaine autorisé.', NULL, NULL, NULL, NULL),
('activite_credit', 'DOMAINE', 'activites_credit', 'La valeur de code activité est hors domaine autorisé.', NULL, NULL, NULL, NULL),
('type_credit', 'DOMAINE', 'types_credit', 'La valeur de type crédit est hors domaine autorisé.', NULL, NULL, NULL, NULL),
('classe_retard', 'DOMAINE', 'classes_retard', 'La valeur de classe retard est hors domaine autorisé.', NULL, NULL, NULL, NULL), --marche
('situation_credit', 'DOMAINE', 'situations_credit', 'La valeur de situation crédit est hors domaine autorisé.', NULL, NULL, NULL, NULL),
('duree_initiale', 'DOMAINE', 'durees_credit', 'La valeur de durée initiale est hors domaine autorisé.', NULL, NULL, NULL, NULL),
('duree_restante', 'DOMAINE', 'durees_credit', 'La valeur de durée restante est hors domaine autorisé.', NULL, NULL, NULL, NULL),
('code_wilaya', 'DOMAINE', 'wilayas', 'La valeur de code wilaya est hors domaine autorisé.', NULL, NULL, NULL, NULL),


('code_wilaya', 'DOIT_ETRE_NULL_SI', NULL, 'Le code wilaya ne doit pas être renseigné si le pays n’est pas l’Algérie.', 'code_pays', '!=012', NULL, NULL),
('classe_retard', 'DOIT_ETRE_NULL_SI', NULL, 'Classe retard ne doit pas être renseignée si situation_credit est dans [001,002,020,900,014,018,005].', 'situation_credit', '001,002,020,900,014,018,005', NULL, NULL), --marche
('classe_retard', 'DOIT_ETRE_NULL_SI', NULL, 'Classe retard ne doit pas être renseignée si id_plafond est 900.', 'id_plafond', '900', NULL, NULL),
--('mensualite', 'DOIT_ETRE_NULL_SI', NULL, 'La mensualité ne doit pas être renseignée si type_credit n’est pas dans [050,051,052].', 'type_credit', '!=050,051,052', NULL, NULL),


('type_credit', 'VALEURS_INTERDITES_SI', '050,051,052,080', 'Type crédit interdit pour débiteur du type : entreprise (i3).', 'participant_type_cle', 'i3', NULL, NULL),
        
('situation_credit', 'EGAL_A_SI', '900', 'La situation crédit doit être 900 si type_credit est 900.', 'type_credit', '900', 'situation_credit', '900'), 
    

('duree_initiale', 'VALEUR_INTERDITE', '000', 'La durée initiale ne doit pas être 000.', NULL, NULL, NULL, NULL),
('duree_initiale', 'EGAL_A_SI', '999', 'La durée initiale doit être 999 si situation_credit est 001.', 'situation_credit', '001', 'duree_initiale', '999'),
('duree_initiale', 'VALEURS_INTERDITES_SI', '999', 'La durée initiale ne doit pas être 999 si situation_credit n’est pas 001', 'situation_credit', '!=001', NULL, NULL),
('duree_initiale', 'EGAL_A_SI', '900', 'La durée initiale doit être 900 si type_credit est 900.', 'type_credit', '900', 'duree_initiale', '900'),
('duree_initiale', 'VALEURS_INTERDITES_SI', '900', 'La durée initiale ne doit pas être 900 si type_credit n’est pas 900', 'type_credit', '!=900', NULL, NULL),

('duree_restante', 'EGAL_A_SI', '999', 'La durée restante doit être 999 si le crédit est rejeté (situation_credit=001).', 'situation_credit', '001', 'duree_restante', '999'),
('duree_restante', 'EGAL_A_SI', '900', 'La durée restante doit être 900 si type_credit est 900.', 'type_credit', '900', 'duree_restante', '900'),

('credit_accorde', 'EGAL_A_SI', '0', 'Le crédit accordé doit être 0 si le crédit est rejété (situation=001).', 'situation_credit', '001', 'credit_accorde', '0'),

('credit_accorde', 'SUP_A_SI', '0', 'Le crédit accordé doit être supérieur à 0 quand plafond n''est pas renséigné', 
 'id_plafond', 'null', 
 'credit_accorde', '0'),

('solde_restant', 'EGAL_A_SI', '0', 'Le solde restant doit être 0 si type_credit n’est pas 900 et situation_credit est 900.', 'type_credit,situation_credit', '!=900,900', 'solde_restant', '0'),
('solde_restant', 'EGAL_A_SI', '0', 'Le solde restant doit être 0 si situation_credit est egal à l''un de ces 4 : [001,020,005,018].', 'situation_credit', '001,020,018', 'solde_restant', '0'),

('cout_total_credit', 'DOIT_ETRE_NULL_OU_ZERO_SI', NULL, 'Le coût total doit être 0 si type_credit dans [050,051,052] ET situation_credit dans [001,005,020,018]', 
 'type_credit,situation_credit', 
 '050,051,052|001,005,020,018', 
 NULL, NULL),
('mensualite', 'DOIT_ETRE_NULL_OU_ZERO_SI', NULL, 'La mensualité doit être 0 si type_credit dans [050,051,052] et situation_credit dans [001,005,020,018].', 'type_credit,situation_credit', '050,051,052|001,005,020,018', NULL, NULL),
('montant_garantie', 'SUP_A_SI', '0', 'Le montant de la garantie doit être supérieur à 0 pour type_garantie != 999.', 'type_garantie', '!=999', NULL, NULL),
('montant_garantie', 'EGAL_A_SI', '0', 'Le montant de la garantie doit être 0 si type_garantie est 999.', 'type_garantie', '999', 'montant_garantie', '0'),
('code_agence', 'VERIFIER_WILAYA_AGENCE', NULL, 'Le code agence ne correspond pas à la wilaya renseignée.', 'code_wilaya', NULL, NULL, NULL);

-- #endregion
INSERT INTO parametrage (parametre, valeur) VALUES
('sequence_dccr_actuelle', 1);


INSERT INTO utilisateurs (
    matricule,
    nom_complet,
    email,
    mot_de_passe,
    role
) VALUES (
    'anis2002',  
    'Alim Anis', 
    'bot.dccr.sga@gmail.com',
    'vxCzvWBe9Tj0IDi+ME5iWrCd+ky+2KaidxEyerW/3a3c2ZU4zdCI/iJTvBVqlFXRbP4ZxWf7EdZwImoVJ/xSkA==', 
    1
);


INSERT INTO tableau_de_bord (description_kpi, requete_sql)
VALUES
-- KPI 1: Volume des crédits et montants moyens par mois et année
('Volume des crédits et montants moyens par mois et année',
'SELECT
    FORMAT(date_declaration, ''yyyy-MM'') AS Periode,
    COUNT(*) AS VolumeCredits,
    AVG(credit_accorde) AS MontantMoyenAccorde
FROM
    credits
GROUP BY
    FORMAT(date_declaration, ''yyyy-MM'')
ORDER BY
    Periode DESC;'),

-- KPI 2a: Distribution par Type de Crédit
('Distribution par Type de Crédit',
'SELECT
    tc.domaine AS TypeDeCredit,
    COUNT(c.numero_contrat_credit) AS NombreDeCredits
FROM
    credits c
JOIN
    types_credit tc ON c.type_credit = tc.code
GROUP BY
    tc.domaine
ORDER BY
    NombreDeCredits DESC;'),

-- KPI 2b: Distribution par Activité de Crédit
('Distribution par Activité de Crédit',
'SELECT
    ac.domaine AS Activite,
    COUNT(c.numero_contrat_credit) AS NombreDeCredits
FROM
    credits c
JOIN
    activites_credit ac ON c.activite_credit = ac.code
GROUP BY
    ac.domaine
ORDER BY
    NombreDeCredits DESC;'),

-- KPI 2c: Distribution par Classe de Retard
('Distribution par Classe de Retard',
'SELECT
    ISNULL(cr.domaine, ''Pas de Retard'') AS ClasseDeRetard,
    COUNT(c.numero_contrat_credit) AS NombreDeCredits
FROM
    credits c
LEFT JOIN
    classes_retard cr ON c.classe_retard = cr.code
GROUP BY
    ISNULL(cr.domaine, ''Pas de Retard'')
ORDER BY
    NombreDeCredits DESC;'),

-- KPI 2d: Distribution par Situation du Débiteur
('Distribution par Situation du Débiteur',
'SELECT
    sc.domaine AS SituationDebiteur,
    COUNT(c.numero_contrat_credit) AS NombreDeCredits
FROM
    credits c
JOIN
    situations_credit sc ON c.situation_credit = sc.code
GROUP BY
    sc.domaine
ORDER BY
    NombreDeCredits DESC;'),

-- KPI 3: Volume des déclarations générées ce trimestre et comparaison
('Volume des déclarations générées ce trimestre et comparaison',
'WITH Volumes AS (
    SELECT
        (SELECT COUNT(*) FROM fichiers_xml
         WHERE DATEPART(quarter, date_heure_generation_xml) = DATEPART(quarter, GETDATE())
           AND YEAR(date_heure_generation_xml) = YEAR(GETDATE())) AS VolumeTrimestreActuel,

        (SELECT COUNT(*) FROM fichiers_xml
         WHERE DATEPART(quarter, date_heure_generation_xml) = DATEPART(quarter, DATEADD(quarter, -1, GETDATE()))
           AND YEAR(date_heure_generation_xml) = YEAR(DATEADD(quarter, -1, GETDATE()))) AS VolumeTrimestrePrecedent
)
SELECT
    VolumeTrimestreActuel,
    VolumeTrimestrePrecedent,
    CASE
        WHEN VolumeTrimestrePrecedent > 0
        THEN (CAST(VolumeTrimestreActuel AS float) - VolumeTrimestrePrecedent) / VolumeTrimestrePrecedent * 100
        ELSE NULL
    END AS TauxDeChangementEnPourcentage
FROM
    Volumes;'),

-- KPI 4: Activité la plus récente sur l'application
('Activité la plus récente sur l''application',
'SELECT TOP 1
    pa.date_action,
    u.nom_complet,
    CASE pa.type_action
        WHEN 0 THEN ''Création''
        WHEN 1 THEN ''Modification''
        WHEN 2 THEN ''Suppression''
        WHEN 3 THEN ''Lecture''
        ELSE ''Inconnu''
    END AS TypeAction,
    pa.table_ciblée,
    pa.id_entité
FROM
    pistes_audit pa
JOIN
    utilisateurs u ON pa.matricule_utilisateur = u.matricule
ORDER BY
    pa.date_action DESC;'),

-- KPI 5: Rapport déclarations soumises vs. non soumises
('Rapport déclarations soumises vs. non soumises (à CREM)',
'SELECT
    SUM(CASE WHEN fx.id_fichier_xml IS NOT NULL THEN 1 ELSE 0 END) AS DeclarationsSoumises,
    SUM(CASE WHEN fx.id_fichier_xml IS NULL THEN 1 ELSE 0 END) AS DeclarationsNonSoumises
FROM
    credits c
LEFT JOIN
    fichiers_xml fx ON c.id_excel = fx.id_fichier_excel;'),

-- KPI 6: Top 5 des types de garanties les plus communs
('Top 5 des types de garanties les plus communs',
'SELECT TOP 5
    tg.domaine AS TypeDeGarantie,
    COUNT(*) AS NombreUtilisations
FROM
    garanties g
JOIN
    types_garantie tg ON g.type_garantie = tg.code
GROUP BY
    tg.domaine
ORDER BY
    NombreUtilisations DESC;'),


-- KPI X: Somme des échéances impayées par agence
('Somme des échéances impayées par agence',
'SELECT
    a.domaine AS Agence,
    SUM(c.nombre_echeances_impayes) AS TotalEcheancesImpayees
FROM
    credits c
JOIN
    lieux l ON c.id_lieu = l.id_lieu
JOIN
    agences a ON l.code_agence = a.code
WHERE
    c.nombre_echeances_impayes IS NOT NULL AND c.nombre_echeances_impayes > 0
GROUP BY
    a.domaine
HAVING
    SUM(c.nombre_echeances_impayes) > 0
ORDER BY
    TotalEcheancesImpayees DESC;'),
    
('Crédits utilisés vs Reste à payer',
'SELECT
    ''Crédit utilisé'' AS Libellé,
    SUM(credit_accorde - solde_restant) AS Montant
FROM credits
WHERE credit_accorde > 0 
  AND solde_restant >= 0
  AND credit_accorde >= solde_restant
UNION ALL
SELECT
    ''Reste à payer'' AS Libellé,
    SUM(solde_restant) AS Montant
FROM credits
WHERE credit_accorde > 0 
  AND solde_restant > 0;');