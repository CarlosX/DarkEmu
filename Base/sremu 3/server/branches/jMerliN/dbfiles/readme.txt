To setup the database:

1. Install/setup MySQL:
  i. http://dev.mysql.com/downloads/mysql/5.1.html#downloads  Choose "Windows Essentials"
    This server currently uses MySQL 5.1.
  ii. Run the msi package.
  iii. Choose 'complete' installation.
  iv. After installation is complete, choose to configure the server NOW!
  v. Choose "Standard Configuration"
  vi. Accept the default service settings.
  vii. Input the root password you want (use 'sremu' if you want to use SREmu out-of-the-box) and ONLY ALLOW LOCAL ACCESS (unless you really know wtf you're doing!)

2. Install the MySQL Gui Tools
note: This is a simple setup for people running both the core & auth servers on their local machines.  If you have
a more complex setup involving multiple DB servers, this guide will be quite useless to you.  Cheerio.

  i. Go to http://dev.mysql.com/get/Downloads/MySQLGUITools/mysql-gui-tools-5.0-r15-win32.msi/from/pick#mirrors and pick a mirror.
  ii. Install it (yes, it's that easy you tards!)
  iii. Run the 'Query Browser'
  iv. Type 'CREATE DATABASE `sremu`' and execute it.
  v. Click File->Open Script, and choose the script in the same folder as this readme!
  vi. Before the line that reads "DROP TABLE IF EXISTS `characters`;", type "USE `sremu`;" (without the quotes)
  vii. Run that shit.  Once it's done, your DB is setup and ready to go.

3. Create an unpriveleged account for access!
  i. Open the MySQL Administrator
  ii. Under 'User Administration', click 'Add New User'
  iii. Enter 'sremu' for the username, then enter a password you want and click 'Apply Changes'.
  iv. Click 'Schema Permissions' and click 'sremu' and 'SELECT', 'DELETE', 'INSERT', and 'UPDATE' permissions, then apply.

DONE!