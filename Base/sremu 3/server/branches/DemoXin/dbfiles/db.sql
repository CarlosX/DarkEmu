-- MySQL dump 10.11
--
-- Host: localhost    Database: sremu
-- ------------------------------------------------------
-- Server version	5.0.67-community-nt

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `characters`
--

DROP TABLE IF EXISTS `characters`;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8;
CREATE TABLE `characters` (
  `id` int(10) unsigned NOT NULL auto_increment,
  `account` int(10) unsigned NOT NULL,
  `name` varchar(45) NOT NULL,
  `chartype` int(10) unsigned NOT NULL default '1907',
  `volume` int(10) unsigned NOT NULL default '22',
  `level` int(10) unsigned NOT NULL default '1',
  `experience` int(10) unsigned NOT NULL default '0',
  `strength` int(10) unsigned NOT NULL default '20',
  `intelligence` int(10) unsigned NOT NULL default '30',
  `attribute` int(10) unsigned NOT NULL default '0',
  `hp` int(10) unsigned NOT NULL default '200',
  `mp` int(10) unsigned NOT NULL default '300',
  `deletion_mark` tinyint(1) NOT NULL default '0',
  `deletion_time` int(10) unsigned NOT NULL default '0',
  `gold` int(10) unsigned NOT NULL default '0',
  `sp` int(10) unsigned NOT NULL default '0',
  `gm` tinyint(1) NOT NULL default '0',
  `xsect` int(10) unsigned NOT NULL default '168' COMMENT 'Default loc Jangan',
  `ysect` int(10) unsigned NOT NULL default '98',
  `xpos` int(10) unsigned NOT NULL default '978',
  `ypos` int(10) unsigned NOT NULL default '1097',
  `zpos` int(10) unsigned NOT NULL default '40',
  `cur_hp` int(10) unsigned NOT NULL default '200',
  `cur_mp` int(10) unsigned NOT NULL default '300',
  `min_phyatk` int(10) unsigned NOT NULL default '5',
  `max_phyatk` int(10) unsigned NOT NULL default '10',
  `min_magatk` int(10) unsigned NOT NULL default '5',
  `max_magatk` int(10) unsigned NOT NULL default '10',
  `phydef` int(10) unsigned NOT NULL default '9',
  `magdef` int(10) unsigned NOT NULL default '7',
  `hit` int(10) unsigned NOT NULL default '3',
  `parry` int(10) unsigned NOT NULL default '5',
  `walkspeed` int(10) unsigned NOT NULL default '16',
  `runspeed` int(10) unsigned NOT NULL default '50',
  `berserkspeed` int(10) unsigned NOT NULL default '100',
  `berserking` tinyint(1) NOT NULL default '0',
  `pvp` int(10) unsigned NOT NULL default '255',
  PRIMARY KEY  (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=latin1;
SET character_set_client = @saved_cs_client;

--
-- Dumping data for table `characters`
--

LOCK TABLES `characters` WRITE;
/*!40000 ALTER TABLE `characters` DISABLE KEYS */;
INSERT INTO `characters` VALUES (8,2,'lulz',1907,34,1,0,20,30,0,200,300,0,0,0,0,1,168,98,978,1097,40,200,300,5,10,5,10,9,7,3,5,16,50,100,0,255),(9,3,'root',1907,0,1,0,20,30,0,200,300,0,0,0,0,0,168,98,978,1097,40,200,300,5,10,5,10,9,7,3,5,16,50,100,0,255);
/*!40000 ALTER TABLE `characters` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `items`
--

DROP TABLE IF EXISTS `items`;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8;
CREATE TABLE `items` (
  `id` int(10) unsigned NOT NULL auto_increment,
  `itemtype` int(10) unsigned NOT NULL,
  `owner` int(10) unsigned NOT NULL,
  `plusvalue` int(10) unsigned NOT NULL default '0',
  `slot` int(10) unsigned NOT NULL,
  `type` int(10) unsigned NOT NULL default '0' COMMENT 'CH = 0, ETC = 1',
  `quantity` int(10) unsigned NOT NULL default '1',
  `durability` int(10) unsigned NOT NULL default '30',
  PRIMARY KEY  (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=70 DEFAULT CHARSET=latin1;
SET character_set_client = @saved_cs_client;

--
-- Dumping data for table `items`
--

LOCK TABLES `items` WRITE;
/*!40000 ALTER TABLE `items` DISABLE KEYS */;
INSERT INTO `items` VALUES (62,4940,8,0,3,0,1,30),(63,4868,8,0,1,0,1,30),(64,4796,8,0,13,0,1,30),(65,4976,8,0,5,0,1,30),(66,4760,8,0,0,0,1,30),(67,4904,8,0,4,0,1,30),(68,4832,8,0,2,0,1,30),(69,19622,8,0,6,0,1,30);
/*!40000 ALTER TABLE `items` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `masteries`
--

DROP TABLE IF EXISTS `masteries`;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8;
CREATE TABLE `masteries` (
  `id` int(10) unsigned NOT NULL auto_increment,
  `mastery` int(10) unsigned NOT NULL,
  `slevel` int(10) unsigned NOT NULL default '0',
  `owner` int(10) unsigned NOT NULL,
  PRIMARY KEY  (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=23 DEFAULT CHARSET=latin1;
SET character_set_client = @saved_cs_client;

--
-- Dumping data for table `masteries`
--

LOCK TABLES `masteries` WRITE;
/*!40000 ALTER TABLE `masteries` DISABLE KEYS */;
INSERT INTO `masteries` VALUES (9,257,0,8),(10,258,0,8),(11,259,0,8),(12,273,0,8),(13,274,0,8),(14,275,0,8),(15,276,0,8),(16,257,0,9),(17,258,0,9),(18,259,0,9),(19,273,0,9),(20,274,0,9),(21,275,0,9),(22,276,0,9);
/*!40000 ALTER TABLE `masteries` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `news`
--

DROP TABLE IF EXISTS `news`;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8;
CREATE TABLE `news` (
  `id` int(11) NOT NULL auto_increment,
  `head` varchar(30) default NULL,
  `text` varchar(250) default NULL,
  `day` int(2) default NULL,
  `month` int(2) default NULL,
  PRIMARY KEY  (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=3 DEFAULT CHARSET=latin1;
SET character_set_client = @saved_cs_client;

--
-- Dumping data for table `news`
--

LOCK TABLES `news` WRITE;
/*!40000 ALTER TABLE `news` DISABLE KEYS */;
INSERT INTO `news` VALUES (1,'Opening','<center><font color=red><b>Welcome to the SREmu C++ Project</b></font></center>',28,12);
/*!40000 ALTER TABLE `news` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `quests`
--

DROP TABLE IF EXISTS `quests`;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8;
CREATE TABLE `quests` (
  `id` int(10) unsigned NOT NULL auto_increment,
  `owner` int(10) unsigned NOT NULL,
  `quest` int(10) unsigned NOT NULL,
  PRIMARY KEY  (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
SET character_set_client = @saved_cs_client;

--
-- Dumping data for table `quests`
--

LOCK TABLES `quests` WRITE;
/*!40000 ALTER TABLE `quests` DISABLE KEYS */;
/*!40000 ALTER TABLE `quests` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `servers`
--

DROP TABLE IF EXISTS `servers`;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8;
CREATE TABLE `servers` (
  `id` int(10) unsigned NOT NULL auto_increment,
  `name` varchar(45) NOT NULL,
  `users_current` int(10) unsigned NOT NULL default '0',
  `users_max` int(10) unsigned NOT NULL default '500',
  `state` int(10) unsigned NOT NULL default '1',
  `ip` varchar(45) NOT NULL,
  `port` int(10) unsigned NOT NULL default '15000',
  PRIMARY KEY  (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=latin1;
SET character_set_client = @saved_cs_client;

--
-- Dumping data for table `servers`
--

LOCK TABLES `servers` WRITE;
/*!40000 ALTER TABLE `servers` DISABLE KEYS */;
INSERT INTO `servers` VALUES (3,'SRVC++',0,500,1,'127.0.0.1',15780),(4,'Remote',0,500,1,'5.145.168.243',15780);
/*!40000 ALTER TABLE `servers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8;
CREATE TABLE `users` (
  `id` int(10) unsigned NOT NULL auto_increment,
  `username` varchar(45) NOT NULL,
  `password` varchar(45) NOT NULL,
  `failed_logins` int(10) unsigned NOT NULL default '0',
  PRIMARY KEY  (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=latin1;
SET character_set_client = @saved_cs_client;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (2,'clearscreen','test',0),(3,'nick','test',0);
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2009-01-02 18:42:49
