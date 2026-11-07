# Challenge 1

Dit is een monorepo met:

| Map | Wat |
|---|---|
| `backend/` | ASP.NET Core API (`TicketService.API`) + Bruno tests |
| `frontend/ConductorTicketService/` | MAUI app voor de conducteur (QR-code en gezicht scannen) |
| `frontend/PassengerTicketService/` | MAUI app voor de reiziger |
| `postgresDatabase/` | Docker Compose voor PostgreSQL + Adminer |

## Wat je nodig hebt

- .NET 10 SDK
- MAUI Android workload: `dotnet workload install maui-android`
- Docker (met Docker Compose)
- Een Android-toestel of emulator
- Een Azure **Face** resource (de gratis F0-tier is genoeg)
- `openssl` om de JWT-sleutel te genereren
- [ngrok](https://ngrok.com/) met een (gratis) account, zodat de apps de API via HTTPS bereiken
- [Bruno](https://www.usebruno.com/) als je de API-tests wilt draaien (optioneel)

## 1. Database starten

```bash
cd postgresDatabase
cp .env.example .env
```

Vul in `.env` een gebruikersnaam en wachtwoord in (`DB_USER`, `DB_PASSWORD`). Start daarna de containers:

```bash
docker compose up -d
```

Adminer (database bekijken in de browser) draait op <http://localhost:7777>.

## 2. User secrets van de API instellen

Alle geheimen staan in user secrets, niet in de repo. Een voorbeeld staat in `backend/TicketService.API/secrets.example.json`.

```bash
cd backend/TicketService.API

dotnet user-secrets set "ConnectionStrings:Docker" "Host=localhost;Port=5432;Database=ticketservice;Username=<DB_USER>;Password=<DB_PASSWORD>"

dotnet user-secrets set "Face:Endpoint" "https://<jouw-resource>.cognitiveservices.azure.com/"
dotnet user-secrets set "Face:ApiKey" "<key uit de Azure portal>"
dotnet user-secrets set "Face:GroupId" "<kies een naam, bv. ticketservice-jouwnaam>"

dotnet user-secrets set "Jwt:Key" "$(openssl rand -base64 64)"
```

- **Database**: gebruik dezelfde gebruiker en hetzelfde wachtwoord als in `.env`. De database zelf wordt in stap 3 aangemaakt.
- **Face**: endpoint en key vind je in de Azure portal bij je Face resource onder *Keys and Endpoint*. Gebruik enkel het adres tot en met `.com`, zonder pad erachter. Of er een `/` op het einde staat, maakt niet uit.
- **Face:GroupId**: kleine letters, cijfers, `-` en `_`. Kies een eigen naam als je een Azure resource deelt met iemand anders, anders wissen jullie elkaars gezichten. De groep wordt automatisch aangemaakt in stap 4.
- **Jwt:Key**: de sleutel waarmee de login-tokens ondertekend worden. Elk teamlid mag een eigen sleutel hebben. Verander je hem, dan moet iedereen opnieuw inloggen.

Issuer, audience en de geldigheid van een token (14 dagen) staan in `appsettings.json`. Die hoef je niet in te stellen.

## 3. Database aanmaken (migraties)

`dotnet-ef` staat als lokale tool in de repo. Vanuit de root van de repo:

```bash
dotnet tool restore

cd backend
dotnet ef database update --project TicketService.Infrastructure --startup-project TicketService.API
```

Had je al een database van vóór de login-functionaliteit? Verwijder die dan eerst, want de wachtwoorden zijn nu gehasht en de oude werken niet meer:

```bash
dotnet ef database drop --project TicketService.Infrastructure --startup-project TicketService.API
```

## 4. API starten en testdata laden

```bash
cd backend/TicketService.API
dotnet run --launch-profile http
```

Het `https`-profiel werkt ook. De apps gebruiken de API toch via ngrok (stap 5), en ngrok zorgt voor de HTTPS.

Laad daarna de testdata. Dit wist ook alle gezichten in Azure en maakt de Azure-groep (opnieuw) aan:

```bash
curl -X POST http://localhost:5271/api/dev/reset
```

Swagger staat op <http://localhost:5271/swagger>.

### Testaccounts

Het wachtwoord is voor alle accounts `password`.

| E-mail | Rol |
|---|---|
| `conductor@test.be` | Conducteur |
| `nick@test.be`, `sarah@test.be`, `tom@test.be` | Reiziger |

Je kunt ook zelf een account aanmaken: in de reizigersapp via *Account aanmaken* op het startscherm, in de conducteursapp via *Registreren* op het loginscherm. Via de API gaat het met `POST /api/auth/passenger/register` en `POST /api/auth/conductor/register`.
Een conducteur heeft daarbij een registratiecode nodig. In het echt zou die in een uitnodigingsmail staan; voor de demo staat hij in `appsettings.json` onder `Registration:ConductorCode` (standaard `CONDUCTOR2026`).

De tickets in de testdata hebben vaste QR-codes: `00000000-0000-0000-0000-00000000000N`, waarbij N het ticket-id is (1 t.e.m. 5).

## 5. ngrok starten (HTTPS voor de apps)

De apps praten alleen via HTTPS met de API. ngrok geeft een publiek HTTPS-adres met een geldig certificaat en stuurt het verkeer door naar de API op je eigen pc:

```
Telefoon ──HTTPS──▶ https://<jouw-domein>.ngrok-free.dev ──▶ ngrok op je pc ──▶ localhost:5271 (API)
```

Eenmalig:

1. Maak een gratis account op [ngrok.com](https://ngrok.com/).
2. Koppel je account aan ngrok op je pc. Je token staat op het dashboard onder *Your Authtoken*.
   ```bash
   ngrok config add-authtoken <jouw-token>
   ```
3. Zoek je vaste, gratis domein op het [dashboard](https://dashboard.ngrok.com) onder **Domains**. Dat domein blijft altijd hetzelfde.

Elke keer dat je de API gebruikt (laat de API zelf ook draaien):

```bash
ngrok http 5271 --url=https://<jouw-domein>.ngrok-free.dev
```

Test of het werkt:

```bash
curl -i -X POST https://<jouw-domein>.ngrok-free.dev/api/auth/conductor/login \
  -H "Content-Type: application/json" \
  -d '{"email":"conductor@test.be","password":"password"}'
```

Je krijgt `HTTP/2 200` met een `token`.

> De API leest de `X-Forwarded-*`-headers van ngrok (`UseForwardedHeaders` in `Program.cs`). Zo weet hij dat het oorspronkelijke verzoek HTTPS was, en stuurt hij niet door naar een andere poort.

## 6. Apps op je telefoon

1. **Kies de URL van de API.** Beide apps halen die uit één bestand: `frontend/Directory.Build.props`. Daar staat de URL van de ngrok-tunnel die we voor de demo gebruiken.

   Wil je je eigen ngrok-domein gebruiken? Pas `Directory.Build.props` dan niet aan, maar maak een eigen bestand (dat staat niet in git):
   ```bash
   cd frontend
   cp ApiBaseUrl.local.props.example ApiBaseUrl.local.props
   ```
   Zet je eigen domein in `ApiBaseUrl.local.props`. Gebruik altijd `https://`, want de apps weigeren gewone `http://`.
2. **Build beide apps opnieuw na het wijzigen van de URL.** De URL wordt tijdens het builden in de app gezet. Ziet je IDE de wijziging niet, herlaad dan het project.
3. **Had je de apps al eens geïnstalleerd vóór de login-functionaliteit?** Verwijder ze dan eerst één keer van je telefoon. De opgeslagen gegevens van de oude versie kunnen niet meer gelezen worden.

De telefoon hoeft niet op hetzelfde wifi-netwerk te zitten als je pc. Mobiele data werkt ook, want alles gaat via ngrok.

## Bruno testing

De collectie staat in `backend/bruno`. Open die map in Bruno, kies de environment **Local** en gebruik **Run Collection** (`…` naast de collectienaam → *Run*). Alles loopt in volgorde: `01 Reset database` begint telkens met schone testdata, en de `Login - ...`-requests zetten de token voor de requests die erna komen.

- Een volledige run duurt ongeveer 1,5 minuut. Requests die Azure gebruiken wachten 6 seconden, omdat de gratis Face-tier maar 20 calls per minuut toelaat. Krijg je toch een `429`, wacht dan een minuut en verhoog de wachttijd in `collection.bru`.
- Je moet zelf twee afbeeldingen voorzien in `backend/bruno/images/`. Die staan niet in git.
  - `nick.jpg`: een foto met één gezicht. Claude heeft de tests geschreven en heeft dus overal "Nick" gebruikt. Je mag gewoon een foto van jezelf gebruiken onder die naam.
  - `no-face.png`: een foto waar geen gezicht op staat.
