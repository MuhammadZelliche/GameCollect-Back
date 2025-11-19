erDiagram
USER {
int UserId PK
string Username
string Email
string PasswordHash
datetime DateCreation
string Role
}

    GAME {
        int GameId PK
        string Titre
        string Plateforme
        int AnneeSortie
        string ImageUrl
        string Rarete
    }

    USERGAME {
        int UserGameId PK
        int UserId FK
        int GameId FK
        datetime DateAjout
        int NotePerso
    }

    REVIEW {
        int ReviewId PK
        int UserId FK
        int GameId FK
        int Note
        string Commentaire
        datetime DatePublication
    }

    USER ||--o{ USERGAME : "possède"
    GAME ||--o{ USERGAME : "appartient à"
    USER ||--o{ REVIEW : "écrit"
    GAME ||--o{ REVIEW : "reçoit"
