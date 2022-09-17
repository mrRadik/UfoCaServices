create table "Certificates"
(
    "Id"         integer default nextval('certificates_id_seq'::regclass) not null
        constraint certificates_pkey
            primary key,
    "CertId"     integer                                                  not null
        constraint certificates_certid_key
            unique,
    "Issuer"     varchar(255)                                             not null,
    "NotAfter"   bigint                                                   not null,
    "NotBefore"  bigint                                                   not null,
    "Serial"     varchar(50)                                              not null,
    "Subject"    varchar                                                  not null,
    "Thumbprint" varchar(50)                                              not null,
    "Data"       varchar                                                  not null
);