
# DailyQuizAPI

**DailyQuizAPI** est une API REST développée avec **.NET 8**, conçue pour alimenter un jeu de type Wordle nommé **Sumot**.  Je pensais faire une seule API pour plusieurs applications de jeux, d'où le nom et certaines fonctionnalités générales qui au final ne servent pas.
Ce projet personnel a pour objectif de renforcer mes compétences en **.NET / EF Core / PostgreSQL / Azure** et démontre une architecture scalable, moderne et maintenable.

## 🧠 Objectif

Ce projet permet de :

- Obtenir les **mots du jour** (qu'on appelle Sumot)
- Gérer l'**authentification** via Identity (JWT)
- Maintenir un **historique de parties**
- Gérer une **liste d'amis**
- Exécuter des **tâches planifiées** avec Hangfire (ex. : choix du mot du jour, nettoyage des utilisateurs inactifs)

## ⚙️ Stack technique

| Composant             | Détail                                       |
|----------------------|-----------------------------------------------|
| **Langage**          | C#                                            |
| **Framework**        | .NET 8 (Minimal API)                          |
| **ORM**              | Entity Framework Core 8                       |
| **Base de données**  | PostgreSQL 17                                 |
| **Authentification** | Identity / JWT Token                          |
| **Logging**          | Serilog                                       |
| **Jobs récurrents**  | Hangfire                                      |
| **Déploiement**      | Azure Web Apps                                |
| **CI/CD**            | GitHub Actions                                |

---

## 📦 Architecture du projet

Le projet est réalisé en Vertical Slice Architecture avec les problématiques communes séparées et mises à la racine et les endpoints / modèles dans leurs propres dossiers à l'intérieur de src/Features

---

🔐 Authentification

Le système d'auth repose sur Identity :

Auth via JWT

Middleware custom .UseAuthentication() pour valider le token

---

⏱️ Jobs Hangfire

Deux jobs récurrents sont définis :

- daily-sumot	02:00	Sélection d'un mot pour le jour
- daily-inactive-check	04:00	Suppression des comptes inactifs

Hangfire utilise la même base PostgreSQL.

---

📊 Logs

Utilisation de Serilog pour journaliser :

Les requêtes entrantes

Les erreurs

Les tâches Hangfire

---

🌐 Déploiement

Déployé sur Azure Web Apps :

Environnement Dev et Prod séparés via branches Neon + chaînes de connexion

CI/CD avec Github Actions

---

👤 Auteur

Maxime Lambert
Développeur full stack
📧 lambert.maxime@protonmail.com
🌐 [LinkedIn](https://www.linkedin.com/in/maximelambert35/)

---

📝 Licence

Ce projet est sous licence MIT.
Tu es libre de l’utiliser, le modifier et le distribuer.
