CREATE TABLE `productorder` (
  `OrderID` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `CartID` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `Created` datetime NOT NULL,
  `Confirmed` datetime NOT NULL,
  `DeliveryDate` datetime NOT NULL,
  `DeliveryTime` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `DeliveryInstructions` varchar(2000) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `PickupDate` datetime NOT NULL,
  `PickupTime` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `ContactName` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `ContactEmail` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `ContactPhone` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `ContactAddress` int(11) NOT NULL,
  `Payment` datetime NOT NULL,
  `PaymentMethod` varchar(12) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `ConfirmationCode` varchar(6) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  PRIMARY KEY (`OrderID`),
  KEY `Email and Confirmation Code` (`ContactEmail`,`ConfirmationCode`) /*!80000 INVISIBLE */,
  KEY `CartID` (`CartID`),
  KEY `DeliveryDate` (`DeliveryDate`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;
