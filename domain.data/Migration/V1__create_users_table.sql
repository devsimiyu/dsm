create type public.user_role as enum ('admin', 'manager', 'staff');

create table if not exists public.users (
	id serial primary key,
	first_name varchar(250) not null,
	last_name varchar(250) not null,
	email varchar(250) not null,
	password varchar(250) not null,
	role user_role not null,
	created_at timestamptz not null default now(),
	updated_at timestamptz,
	deleted_at timestamptz
);

create unique index if not exists user_email_idx on public.users (email);
