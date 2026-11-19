import { Component, OnInit, inject } from '@angular/core';
import { AutocompleteComponent } from '../../shared/components/autocomplete.component';
import { AddressModalComponent } from './components/address-modal.component';
import { AddressesPageFacade } from './data/addresses-page.facade';

@Component({
  selector: 'app-addresses-page',
  standalone: true,
  imports: [AutocompleteComponent, AddressModalComponent],
  providers: [AddressesPageFacade],
  templateUrl: './addresses-page.component.html',
  styleUrl: './addresses-page.component.scss'
})
export class AddressesPageComponent implements OnInit {
  vm = inject(AddressesPageFacade);

  ngOnInit() {
    this.vm.init();
  }
}