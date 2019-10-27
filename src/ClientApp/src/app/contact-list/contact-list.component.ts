import { Component, OnInit } from '@angular/core';
import { ContactService } from 'src/app/contact.service';
import { Contact } from '../contact';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-contact-list',
  templateUrl: './contact-list.component.html',
  styleUrls: ['./contact-list.component.css']
})
export class ContactListComponent implements OnInit {

  private contacts: Contact[];

  private filteredContacts: Contact[];

  constructor(private contactService: ContactService) { }

  ngOnInit() {

    // Request contacts list.

    this.contactService.getList()
      .subscribe(list => {
        this.contacts = list;
        this.filteredContacts = list;
      });
  }

  private onSearchChange(searchText: string): void {

    // Bring search text and values to lower for case-insensitive compare.

    searchText = searchText.toLowerCase();

    // Filter by First or Last Name.

    this.filteredContacts = this.contacts.filter(item => {
      return item.firstName.toLowerCase().indexOf(searchText) != -1 ||
             item.lastName.toLowerCase().indexOf(searchText) != -1;
    });
  }

  private onDeleteClick(item: Contact) {
    this.contactService.delete(item.id).subscribe(
      () => this.removeItem(item)
    )
  }

  private removeItem(item: Contact) {
    for (var i = 0; i < this.contacts.length; i++) {
      if (this.contacts[i] === item) {
        this.contacts.splice(i, 1);
      }
    }

    for (var i = 0; i < this.filteredContacts.length; i++) {
      if (this.filteredContacts[i] === item) {
        this.filteredContacts.splice(i, 1);
      }
    }
  }
}
