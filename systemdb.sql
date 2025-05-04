-- MySQL dump 10.13  Distrib 8.0.41, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: systemdb
-- ------------------------------------------------------
-- Server version	8.0.41

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `candidates`
--

DROP TABLE IF EXISTS `candidates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `candidates` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) NOT NULL,
  `Address` varchar(255) NOT NULL,
  `Age` int DEFAULT NULL,
  `Position` varchar(50) NOT NULL,
  `PartyList` varchar(100) NOT NULL,
  `ElectionId` int NOT NULL,
  `PictureUrl` varchar(255) DEFAULT NULL,
  `Department` varchar(255) DEFAULT NULL,
  `Program` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=57 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `candidates`
--

LOCK TABLES `candidates` WRITE;
/*!40000 ALTER TABLE `candidates` DISABLE KEYS */;
INSERT INTO `candidates` VALUES (47,'Superman','laguna',21,'President','Justice League',1,'/images/img.jpeg','CCS','Bachelor of Science in Computer Science'),(49,'spiderman','laguna',21,'President','Justice League',1,'/images/img.jpeg',NULL,NULL),(50,'spiderman1','laguna',21,'Vice President','Justice League',1,'/images/img.jpeg',NULL,NULL),(51,'batman','laguna',21,'Vice President','Justice League',1,'/images/img.jpeg',NULL,NULL),(52,'robin','laguna',21,'Secretary','Justice League',1,'/images/img.jpeg',NULL,NULL),(53,'ewan','laguna',21,'Secretary','Justice League',1,'/images/img.jpeg',NULL,NULL),(54,'ewan','2121',22,'Treasurer','123',1,'/images/61595433-f1b0-4d48-a8c3-8f02daaf7e77.png',NULL,NULL),(55,'ewan11','2121',22,'President','123',10,'/images/8d9da9b4-7d9a-4f53-8917-00458c03fd62.ico',NULL,NULL),(56,'ewan111','2121',22,'Vice President','123',10,'/images/3f26dbbd-0fea-454e-a21f-34eae49a0b22.ico',NULL,NULL);
/*!40000 ALTER TABLE `candidates` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `elections`
--

DROP TABLE IF EXISTS `elections`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `elections` (
  `ElectionId` int NOT NULL AUTO_INCREMENT,
  `Title` varchar(255) NOT NULL,
  `description` text,
  `Start_time` datetime NOT NULL,
  `End_time` datetime NOT NULL,
  `Created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `Department` varchar(255) DEFAULT NULL,
  `Program` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`ElectionId`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `elections`
--

LOCK TABLES `elections` WRITE;
/*!40000 ALTER TABLE `elections` DISABLE KEYS */;
INSERT INTO `elections` VALUES (1,'Student Leaders Election1',NULL,'2025-05-01 08:00:00','2025-05-31 16:00:00','2025-04-18 06:02:43','ALL',NULL),(10,'student council election',NULL,'2025-05-01 09:43:00','2025-05-10 09:43:00','2025-04-24 01:43:18','CCS','Bachelor of Science in Computer Science');
/*!40000 ALTER TABLE `elections` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `FirstName` varchar(100) NOT NULL,
  `LastName` varchar(100) NOT NULL,
  `Email` varchar(100) NOT NULL,
  `Address` varchar(255) NOT NULL,
  `PasswordHash` varchar(255) NOT NULL,
  `Month` varchar(20) DEFAULT NULL,
  `Day` int DEFAULT NULL,
  `Year` int DEFAULT NULL,
  `UserName` varchar(100) NOT NULL,
  `Age` int DEFAULT NULL,
  `Role` varchar(10) NOT NULL DEFAULT 'User',
  `ResetToken` varchar(255) DEFAULT NULL,
  `TokenExpiry` datetime DEFAULT NULL,
  `Department` varchar(255) DEFAULT NULL,
  `Program` varchar(255) DEFAULT NULL,
  `PhotoPath` varchar(255) DEFAULT NULL,
  `IsApproved` tinyint(1) DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Email` (`Email`),
  UNIQUE KEY `UserName` (`UserName`)
) ENGINE=InnoDB AUTO_INCREMENT=35 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (9,'John Paul','Dimapilis','johnpauldimapilis3@gmail.com','Tuntungin-Putho, Los Banos, Laguna','$2a$11$3KDmkpu0b19Pv1zCFaDrk.K31AkaHy6tOhvxeXiY9ECqGz9w9oq72','May',31,2003,'dimapilisjp',21,'Admin','a9703918-39a2-45fa-9acc-f2a0d049514d','2025-04-30 01:58:36','CCS','Bachelor of Science in Computer Science',NULL,1),(16,'adasdf','asdfasdf','asdfasdf@gmail.com','tuntungin','$2a$11$cUD0yAZ720AcqGn/EPzHouts8P7YI8xwRI.DSxyZGbn/SOOxal/By','September',21,2001,'dimapilisjp2',23,'User',NULL,NULL,'CCS','Bachelor of Science in Computer Science',NULL,1),(18,'adasdf','asdfasdf','dimapilis.johnpaul2211ds@gmail.com','tuntungin','$2a$11$L2Y1Si70HViBBzxqHhFtGulEjRuZ8g.bLDrN5L0giCmzC6wJzI1VG','September',21,2001,'dimapilisjp3',23,'User','dfb7af6f-2a6b-4b35-9499-f39664002bd4','2025-05-04 22:38:06','CCS','Bachelor of Science in Information Technology',NULL,1),(27,'adasdf','asdfasdf','dimapilis.johnpaul221111ds@gmail.com','2121','$2a$11$xnKcahhtCA.J8bu1Beb3TOQWF4U0z42pTICPStYCu.c74NeWgsCX6','December',20,2005,'dimapilisjp9',19,'User',NULL,NULL,'CCS','Bachelor of Science in Computer Science','/images/28196ac4-c705-42f9-9956-909c281c3d42.ico',1),(28,'adasdf','asdfasdf','dimapilis.johnpaul22111111ds@gmail.com','2121','$2a$11$y79OHG.j0VBVucWcpR3m6eultkflH9osQRcsSZ1Q5w4I8T3ABDxOm','December',20,2005,'dimapilisjp91',19,'User',NULL,NULL,'CCS','Bachelor of Science in Computer Science','/images/5bc1aa7c-06bd-4035-8fe4-0a6047788569.ico',1),(29,'adasdf','asdfasdf','dimapilis.johnpaul221111411ds@gmail.com','2121','$2a$11$UglrNgL19.k62pgPMeJgWeSxDkVAhMd7gZwh3ba17k0///Uw6EO0i','December',20,2005,'dimapilisjp911',19,'User',NULL,NULL,'CoEng','Bachelor of Science in Mechanical Engineering','/images/73dc90a8-5fd6-4146-b40c-8e2a7b69804e.ico',1),(30,'adasdf','asdfasdf','dimapilis.johnpaul22111211411ds@gmail.com','2121','$2a$11$GGVP1NA65Z/nED4VTcZod.JXpHFgpPE7OEGQM2d4QmLsnAE4YM5fa','December',20,2005,'dimapilisjp9112',19,'User',NULL,NULL,'CBAA','Bachelor of Science in Entrepreneurship','/images/36eb3298-db13-4f4c-99d6-bb56c4920f4d.ico',1),(32,'adasdf','asdfasdf','dimapilis.johnpaul22123111211411ds@gmail.com','2121','$2a$11$0lZTeBUZnzxtWjm/GsMPrOqj5Y/9GXBJHVYeNWVHyqmDd/AT64i3.','December',20,2005,'dimapilisjp99',19,'User',NULL,NULL,'CAS','Bachelor of Science in Psychology','/images/bdf660e6-64ef-46a5-95d4-e30e7689b0ff.ico',1),(34,'adasdf','asdfasdf','dimapilis.johnpaul2212311881123211411ds@gmail.com','2121','$2a$11$l5N0dNtrHCoIrVxovcMvf.SPTCNfKp7/tNpw128vO0Bi/jMaOR5OS','December',20,2005,'dimapilisjp991233',19,'User',NULL,NULL,'CoEd','Bachelor of Secondary Education','/images/6e46a200-8836-4906-85d6-5b8b567693cc.ico',0);
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `votecast`
--

DROP TABLE IF EXISTS `votecast`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `votecast` (
  `vote_id` int NOT NULL AUTO_INCREMENT,
  `president` varchar(50) NOT NULL,
  `vice_president` varchar(50) NOT NULL,
  PRIMARY KEY (`vote_id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `votecast`
--

LOCK TABLES `votecast` WRITE;
/*!40000 ALTER TABLE `votecast` DISABLE KEYS */;
INSERT INTO `votecast` VALUES (1,'Superman','Spider-Man');
/*!40000 ALTER TABLE `votecast` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `votes`
--

DROP TABLE IF EXISTS `votes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `votes` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` int NOT NULL,
  `ElectionId` int NOT NULL,
  `PresidentCandidateId` int DEFAULT NULL,
  `VicePresidentCandidateId` int DEFAULT NULL,
  `SecretaryCandidateId` int DEFAULT NULL,
  `TreasurerCandidateId` int DEFAULT NULL,
  `AuditorCandidateId` int DEFAULT NULL,
  `PROCandidateId` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `unique_vote_per_election` (`UserId`,`ElectionId`),
  KEY `ElectionId` (`ElectionId`),
  CONSTRAINT `votes_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`),
  CONSTRAINT `votes_ibfk_2` FOREIGN KEY (`ElectionId`) REFERENCES `elections` (`ElectionId`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `votes`
--

LOCK TABLES `votes` WRITE;
/*!40000 ALTER TABLE `votes` DISABLE KEYS */;
INSERT INTO `votes` VALUES (1,16,1,47,50,52,54,NULL,NULL),(2,16,10,55,56,NULL,NULL,NULL,NULL),(3,18,1,49,51,52,54,NULL,NULL),(4,27,1,47,50,52,54,NULL,NULL),(5,28,1,47,51,53,54,NULL,NULL);
/*!40000 ALTER TABLE `votes` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-05-04 21:49:37
