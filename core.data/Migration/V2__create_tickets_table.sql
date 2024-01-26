create table if not exists public.tickets (
	id serial primary key,
	user_id integer not null references users (id) on delete cascade,
	title varchar(250) not null,
	description text,
	created_at timestamptz not null default now(),
	updated_at timestamptz,
	deleted_at timestamptz
);

create index if not exists ticket_user_idx on public.tickets (user_id);
