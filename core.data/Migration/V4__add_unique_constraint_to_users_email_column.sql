alter table public.users
add constraint user_email_unique
unique using index user_email_idx;
