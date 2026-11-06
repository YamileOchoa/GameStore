
-- Tabla Developer
CREATE TABLE developer (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE,
    country VARCHAR(60) NOT NULL,
    founded_year INT NOT NULL CHECK (founded_year BETWEEN 1970 AND 2026)
);

-- Tabla Genre
CREATE TABLE genre (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE,
    description VARCHAR(200)
);

-- Tabla Game
CREATE TABLE game (
    id INT IDENTITY(1,1) PRIMARY KEY,
    title VARCHAR(120) NOT NULL,
    price DECIMAL(8, 2) NOT NULL CHECK (price BETWEEN 0 AND 500),
    release_date DATE NOT NULL,
    age_rating CHAR(1) NOT NULL CHECK (age_rating IN ('E', 'T', 'M')),
    key_stock INT NOT NULL CHECK (key_stock BETWEEN 0 AND 100000),
    developer_id INT NOT NULL FOREIGN KEY REFERENCES developer(id),
    genre_id INT NOT NULL FOREIGN KEY REFERENCES genre(id),
    CONSTRAINT uq_game_title_developer UNIQUE (title, developer_id)
);

-- Registros de prueba
INSERT INTO developer (name, country, founded_year) VALUES 
('CD Projekt Red', 'Polonia', 1994),
('Nintendo', 'Japón', 1989),
('Rockstar Games', 'Estados Unidos', 1998),
('FromSoftware', 'Japón', 1986);

INSERT INTO genre (name, description) VALUES 
('RPG', 'Juegos de rol con progresión de personaje'),
('Aventura', 'Exploración y narrativa'),
('Acción', 'Combate y reflejos'),
('Plataformas', 'Saltos y obstáculos');

INSERT INTO game (title, price, release_date, age_rating, key_stock, developer_id, genre_id) VALUES 
('The Witcher 3', 39.99, '2015-05-19', 'M', 500, 1, 1),
('Cyberpunk 2077', 59.99, '2020-12-10', 'M', 800, 1, 1),
('Zelda: Breath of the Wild', 59.99, '2017-03-03', 'T', 300, 2, 2),
('Super Mario Odyssey', 49.99, '2017-10-27', 'E', 400, 2, 4),
('Red Dead Redemption 2', 59.99, '2018-10-26', 'M', 600, 3, 3),
('Elden Ring', 59.99, '2022-02-25', 'M', 700, 4, 1);