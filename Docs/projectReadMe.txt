** ToDo Server **

o Entity Framework integrieren / Datenbanken optimieren >> DONE!

o Autorisierung einbinden > DONE!

o Kommunikation Client/Server herstellen > DONE!

o Demoapplikation serverseitig entsprechend Ablaufplan > (1)

o Routing > DONE

o GitHub > (1)

o EMail funktionalitaet > DONE

o Account Confirmation	> DONE

o Ueberblick ueber VORTEILE von MVC verschaffen > (3)

o Passwort Reset Funktionalität >

o Account Confirmation Resend Link >

o Client neu schreiben  > (5)

>>CREATION<<

o Ueberblick ueber Exception Handling verschaffen

o Ueberblick ueber Unit testing verschaffen

o E-Mails auf SendGrid o.ae. umziehen / Senden async machen / 
  Exception Handling integrieren / SMTP Login Informationen 
  aus Code entfernen > (2)

o Cross-Origin entfernen

o Kommunikation per SSL implementieren

o Ueberfluessige Sachen aus MVC template entfernen / Help Page deaktivieren

o Ueberfluessige Actions aus Controllern entfernen

o >Ensure your app is reading for production< von der Projekt-Startpage lesen

o Deployment

o Datenbank umbenennen	

o PowerShell Ececution Policy pruefen	


>>PRODUCTION<<

o Cookies, Claims Ueberblick ueber ThirdParty Authorisierungen verschaffen > (5)

o Custom NotFound() Message mit text implementieren





** ToDo Sonstiges **
o Strategien zur Internationalisierung recherchieren (Simon)





** BuildingFromScratch **

* PROJEKT ANLEGEN
o Ordnerstruktur vorbereiten
o Empty Solution anlegen
o ASP.NET WebApi Projekt anlegen > Individual Accounts waehlen
o Zugriff auf Authorisierungsfunktionen per Postman testen

* ENTITY FRAMEWORK INTEGRIEREN (http://www.asp.net/web-api/overview/data/using-web-api-with-entity-framework/part-3)
o  Modelle fuer FeedbackSessions und FeedbackQuestions angelegt. Inhalte aus vorheriger Version kopiert
o Controller erstellen lassen > WICHTIG: Bereits vorhandenen DBContext verwenden. Keinen neuen anlegen
o Enable-Migrations ausfuehren
o Configuration.cs mit Daten fuellen
o Add-Migration und Update-Database ausfuehren
o Zugriff auf Datenbanken per Postman ueberpruefen
o Anpassen der Datenbanken (Verknuepfungen ueber FKs, Generierung von GUIDs,...)
o 



