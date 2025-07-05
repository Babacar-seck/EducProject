# Guide d'Installation - EducProject

## Prérequis

### Backend (.NET 8)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) ou [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [Visual Studio Code](https://code.visualstudio.com/)

### Frontend (React)
- [Node.js](https://nodejs.org/) (version 18 ou supérieure)
- [npm](https://www.npmjs.com/) (inclus avec Node.js)

## Installation

### 1. Cloner le projet
```bash
git clone <repository-url>
cd EducProject
```

### 2. Configuration de la base de données

#### Option A: Utiliser SQL Server Management Studio
1. Ouvrez SQL Server Management Studio
2. Connectez-vous à votre instance SQL Server
3. Ouvrez le fichier `Database/CreateDatabase.sql`
4. Exécutez le script

#### Option B: Utiliser sqlcmd
```bash
sqlcmd -S (localdb)\mssqllocaldb -i Database/CreateDatabase.sql
```

### 3. Configuration du Backend

1. Naviguez vers le dossier Backend :
```bash
cd Backend
```

2. Restaurez les packages NuGet :
```bash
dotnet restore
```

3. Vérifiez la connexion à la base de données dans `appsettings.json` :
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EducProjectDB;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

4. Lancez l'API :
```bash
dotnet run
```

L'API sera accessible sur `https://localhost:7001`

### 4. Configuration du Frontend

1. Ouvrez un nouveau terminal et naviguez vers le dossier Frontend :
```bash
cd Frontend
```

2. Installez les dépendances :
```bash
npm install
```

3. Lancez l'application React :
```bash
npm start
```

L'application sera accessible sur `http://localhost:3000`

## Utilisation

### Comptes par défaut

#### Administrateur
- **Username:** admin
- **Password:** admin123

### Création de comptes

1. **Créer un compte parent :**
   - Allez sur `http://localhost:3000/register`
   - Remplissez le formulaire avec le rôle "Parent"
   - Notez l'ID du compte créé

2. **Créer un compte enfant :**
   - Allez sur `http://localhost:3000/register-child`
   - Remplissez le formulaire en utilisant l'ID du parent

3. **Se connecter :**
   - Allez sur `http://localhost:3000/login`
   - Utilisez les identifiants créés

## API Endpoints

### Authentification
- `POST /api/auth/login` - Connexion
- `POST /api/auth/register` - Inscription utilisateur
- `POST /api/auth/register-child` - Inscription enfant
- `GET /api/auth/validate` - Validation du token

### Utilisateurs
- `GET /api/users` - Liste des utilisateurs
- `GET /api/users/{id}` - Détails d'un utilisateur
- `PUT /api/users/{id}` - Modifier un utilisateur
- `DELETE /api/users/{id}` - Supprimer un utilisateur
- `GET /api/users/children/{parentId}` - Enfants d'un parent

## Dépannage

### Erreurs courantes

1. **Erreur de connexion à la base de données :**
   - Vérifiez que SQL Server est démarré
   - Vérifiez la chaîne de connexion dans `appsettings.json`

2. **Erreur de certificat HTTPS :**
   - Acceptez le certificat de développement ou utilisez HTTP

3. **Erreur de dépendances frontend :**
   - Supprimez le dossier `node_modules` et relancez `npm install`

4. **Erreur de build .NET :**
   - Vérifiez que .NET 8 SDK est installé
   - Relancez `dotnet restore`

## Structure du projet

```
EducProject/
├── Backend/                 # API C# (.NET 8)
│   ├── Controllers/         # Contrôleurs API
│   ├── Data/               # Contexte Entity Framework
│   ├── Models/             # Modèles de données
│   ├── Services/           # Services métier
│   └── Program.cs          # Point d'entrée
├── Frontend/               # Application React
│   ├── src/
│   │   ├── components/     # Composants React
│   │   ├── contexts/       # Contextes React
│   │   ├── services/       # Services API
│   │   └── types/          # Types TypeScript
│   └── public/             # Fichiers publics
├── Database/               # Scripts SQL
└── docs/                   # Documentation
```

## Prochaines étapes

1. **Épic 2 - Catalogue de cours :** Créer les modèles et API pour les cours
2. **Épic 3 - Système de progression :** Implémenter le suivi des progrès
3. **Épic 4 - Interface d'administration :** Créer l'interface d'administration des contenus 