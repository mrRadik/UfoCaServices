create table "Logs"
(
    "Id"          integer default nextval('logs_id_seq'::regclass) not null
        constraint logs_pkey
            primary key,
    "Type"        varchar(50)                                      not null,
    "Application" varchar(50)                                      not null,
    "Message"     varchar(255)                                     not null,
    "Date"        timestamp                                        not null
);