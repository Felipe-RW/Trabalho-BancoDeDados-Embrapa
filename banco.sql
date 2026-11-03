create database if not exists cadastro_db character set utf8mb4;

use cadastro_db;

create table if not exists usuarios(
    id int auto_increment primary key,
    nome varchar(100) not null,
    email varchar(100) not null unique,
    senha varchar(64) not null,
    matricula varchar(50) null,
    ativo boolean not null default true
);

insert ignore into usuarios(nome, email, senha, matricula, ativo)
values('Administrador', 'admin@admin.com',
        '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918',
        '0000', true);