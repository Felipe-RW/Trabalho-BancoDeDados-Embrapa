create database if not exists der character set utf8mb4;
use der;

create table if not exists usuario (
    id int auto_increment primary key,
    nome varchar(100) not null,
    email varchar(100) not null unique,
    senha varchar(50) not null,
    matricula int not null,
    ativo boolean not null default 1
);

create table if not exists categoria (
    id int auto_increment primary key,
    nome varchar(100) not null
);

create table if not exists acao (
    id int auto_increment primary key,
    titulo varchar(100) not null,
    descricao text not null,
    data_de_realizacao date not null,
    status_acao enum('Aprovado', 'Negado', 'Em Avaliação') not null default 'Em Avaliação',
    categoria_id int not null,
    foreign key (categoria_id) references categoria(id)
);

create table if not exists cargo (
    id int auto_increment primary key,
    titulo varchar(100) not null
);

create table if not exists usuario_cargo (
    id int auto_increment primary key,
    usuario_id int not null,
    cargo_id int not null,
    foreign key (usuario_id) references usuario(id),
    foreign key (cargo_id) references cargo(id)
);

create table if not exists acao_usuario (
    usuario_id int not null,
    acao_id int not null,
    foreign key (usuario_id) references usuario(id),
    foreign key (acao_id) references acao(id)
);

create table if not exists validacao (
    motivo_rejeicao varchar(100) not null,
    usuario_id int not null,
    acao_id int not null,
    foreign key (usuario_id) references usuario(id),
    foreign key (acao_id) references acao(id)
);

create table if not exists log_usuario (
    id int auto_increment primary key,
    operacao varchar(100) not null,
    tabela_afetada varchar(100) not null,
    id_registrado int not null,
    data_hora datetime not null,
    matricula_id int not null,
    foreign key (matricula_id) references usuario(id)
);
