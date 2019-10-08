using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlumsailSample.Models;

namespace PlumsailSample.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ContactController : Controller
	{
		private PlumsailDbContext dbContext;

		public ContactController(PlumsailDbContext dbContext)
		{
			this.dbContext = dbContext;
		}

		/// <summary>
		/// ��������� ������ ���������. // GET: api/contact
		/// </summary>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Contact>>> GetContacts()
		{
			return await this.dbContext.Contacts.ToListAsync();
		}

		/// <summary>
		/// ��������� ���������� ��������. // GET: api/contact/1
		/// </summary>
		[HttpGet("{id:int}")]
		public async Task<ActionResult<Contact>> GetContact(int id)
		{
			if (id <= 0)
			{
				return BadRequest();
			}

			// �������� ����� ������ � ��������� ���������������. ���������� ����� Find, ������� ����� ������� ������ �� ������,
			// �� ��������� � ��, ���� ��� ��� ��������� � ��������.

			var contact = await this.dbContext.Contacts.FindAsync(id);

			if (contact == null)
			{
				return NotFound(); // 404
			}

			return contact;
		}

		/// <summary>
		/// ������� ����� �������. // POST: api/contact
		/// </summary>
		[HttpPost]
		public async Task<ActionResult<Contact>> AddContact([FromBody] Contact contact)
		{
			this.dbContext.Contacts.Add(contact);

			await this.dbContext.SaveChangesAsync();

			// ��������� ����� Created*AtAction*, �� ���������� � ������ ��������� Location, ������� ��������� �� ��������� ������.
			// ��� ������ �� ��������� ����� �������������� ��� ��������� �� ��������� ������.

			return CreatedAtAction(nameof(GetContact), new { id = contact.Id }, contact); // 201
		}

		/// <summary>
		/// �������� ������������ �������. // PUT: api/contact/1
		/// </summary>
		[HttpPut("{id:int}")]
		public async Task<ActionResult> UpdateContact(int id, [FromBody] Contact contact)
		{
			if (id <= 0 || id != contact.Id)
			{
				return BadRequest();
			}

			// According to the HTTP specification, a PUT request requires the client to send the entire updated entity, not just the changes.

			this.dbContext.Entry(contact).State = EntityState.Modified;

			try
			{
				await this.dbContext.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException exc)
			{
				// ���� ������ �� ����������, �� �������� ����������
				// DbUpdateConcurrencyException: Attempted to update or delete an entity that does not exist in the store.

				return NotFound(exc.Message); // 404
			}

			// https://www.w3.org/Protocols/rfc2616/rfc2616-sec9.html
			// If an existing resource is modified, either the 200 (OK) or 204 (No Content) response codes SHOULD be sent to indicate successful completion of the request.
			
			return NoContent(); // 204
		}

		/// <summary>
		/// ������� ������������ �������. // DELETE: api/contact/1
		/// </summary>
		[HttpDelete("{id:int}")]
		public async Task<ActionResult> DeleteContact(int id)
		{
			if (id <= 0)
			{
				return BadRequest();
			}

			// �������� ����� ������ � ��������� ���������������. ���������� ����� Find, ������� ����� ������� ������ �� ������,
			// �� ��������� � ��, ���� ��� ��� ��������� � ��������.

			var contact = await this.dbContext.Contacts.FindAsync(id);

			if (contact == null)
			{
				return NotFound(); // 404
			}

			// ������� ������.

			this.dbContext.Contacts.Remove(contact);

			await this.dbContext.SaveChangesAsync();

			// https://www.w3.org/Protocols/rfc2616/rfc2616-sec9.html
			// A successful response SHOULD be 200 (OK) if the response includes an entity describing the status, 202 (Accepted) if the action has not yet been enacted, or 204 (No Content) if the action has been enacted but the response does not include an entity. 

			return NoContent(); // 204
		}
	}
}
