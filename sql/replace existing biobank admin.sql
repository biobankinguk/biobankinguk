-- this script should ONLY be used if:
-- - all admins of the biobank are no longer valid
--     - delete all but one, then use this script to turn the last one into a new admin!
-- - the admin you're changing is ONLY the admin of this one biobank.
--     - if they admin multiples, this will obviously interefere with their other ones.

-- TODO: If and when we fix the registration flow,
--   this script will not be necessarily or useful:
-- The new procedure in that case will be:
-- - User registers their own new account
-- - Old accounts stay as they were
-- - New User is manually granted Biobank Admin rights
--   - by db edit, or User Management GUI or a new CLI command?
-- - They can then fix the admin list themselves

-- lookup by old email
select id, [name], email, PasswordHash, SecurityStamp, UserName from AspNetUsers
where email like '%<old email>%'

-- lookup by organisation
select id, email, o.name from OrganisationUsers ou
join organisations o on o.OrganisationId = ou.OrganisationId
join aspnetusers u on u.Id = ou.OrganisationUserId
where o.Name like '%<org name>%'

-- update the user
update AspNetUsers set
email = '<new email>',
username = '<new email>',
[name] = '<New Name>',
PasswordHash = '',
SecurityStamp = 'MANUAL_PASSWORD_RESET'
where email like '%<old email>%'

-- remove old password restrictions, as it's a "new" account
delete from HistoricalPasswords where UserId = '<user id>'

