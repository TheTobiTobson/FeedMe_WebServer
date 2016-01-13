** ToDo Server **

o Entity Framework integrieren / Datenbanken optimieren >> DONE!

o Autorisierung einbinden > DONE!

o Kommunikation Client/Server herstellen > DONE!

o Routing > DONE

o GitHub > DONE

o EMail funktionalitaet > DONE

o Account Confirmation	> DONE

o Ueberblick ueber VORTEILE von MVC verschaffen > DONE

o Passwort Reset Funktionalität > DONE

o Demoapplikation serverseitig entsprechend Ablaufplan > (1)

o Client neu schreiben  > (5)

>>CREATION<<

o Account Confirmation Resend Link Konzept entwickeln und implementieren>

o Ueberfluessige Sachen aus MVC template entfernen / Help Page deaktivieren

o Ueberfluessige Actions aus Controllern entfernen

o Ueberblick ueber Exception Handling verschaffen

o Ueberblick ueber Unit testing verschaffen

o E-Mails auf SendGrid o.ae. umziehen / 
  Exception Handling integrieren / SMTP Login Informationen 
  aus Code entfernen / Eine zentrale Sendefunktion machen > (2)

o Cross-Origin entfernen

o Kommunikation per SSL implementieren

o >Ensure your app is reading for production< von der Projekt-Startpage lesen

o Deployment

o Datenbank umbenennen	

o PowerShell Excecution Policy pruefen	


>>PRODUCTION<<

o Cookies, Claims Ueberblick ueber ThirdParty Authorisierungen verschaffen > (5)

o Custom NotFound() Message mit text implementieren

o UserManager.ConfirmEmailAsync liefert zurueck, dass Benutzer nicht vorhanden ist. 
  Diese Information sollte fuer potentielle Angreifer entfernt werden.

o Anzahl an fehlgeschlagenen Anmeldeversuchen limitieren



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



