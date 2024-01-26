create table if not exists public.documents (
	id serial primary key,
	ticket_id integer not null references public.tickets (id) on delete cascade,
	name varchar(250) not null,
	path text not null constraint document_path_unique unique,
	created_at timestamptz not null default now(),
	updated_at timestamptz,
	deleted_at timestamptz
);

create index if not exists document_ticket_idx on public.documents (ticket_id);
