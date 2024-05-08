1. Pathway Office 365 Git - this is the one that needs to be modified.

2. LocalAnalyst_Database - contains 5 .sql files to create and populate the required databases.
	a. LocalAnalyst_pmc.sql
	b. LocalAnalyst_pmc080627.sql
	c. LocalAnalyst_pmc080627_pathway.sql
	d. LocalAnalyst_pmc080627_trend.sql
	e. LocalAnalyst_pmccomparision.sql
 
After setting up the MySQL 8.3 database, they can import the data via the MySQL Workbench UI or from the command line
	mysql -u root -p pmc < LocalAnalyst_pmc.sql
	mysql -u root -p pmc080627 < LocalAnalyst_pmc080627.sql
	mysql -u root -p pmc080627 < LocalAnalyst_pmc080627_pathway.sql
	mysql -u root -p pmc080627 < LocalAnalyst_pmc080627_trend.sql
	mysql -u root -p pmccomparision < LocalAnalyst_pmccomparision.sql

3. There are two projects within the solution
	a. Pathway Loader - Reads the UMP60648_212581627201996647.402 and loads it into different pv* tables in pmc080627 
	b. Pathway Report Generator - uses the different pv* tables to create an excel document.