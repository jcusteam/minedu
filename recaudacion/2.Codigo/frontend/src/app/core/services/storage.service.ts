import { Injectable } from '@angular/core';
import { LocalStorageService } from 'ngx-webstorage';

@Injectable({
  providedIn: 'root'
})
export class StorageService {

  constructor(private lStorage: LocalStorageService) {}

  saveValue(key: string, value: string) {
    this.lStorage.store(key, value);
  }

  retrieveValue(key: string) {
    return this.lStorage.retrieve(key);
  }

  removeValue(key: string): void {
    this.lStorage.clear(key);
  }

  clear() {
    this.lStorage.clear();
  }

  observeStorageIten(key: string) {
    return this.lStorage.observe(key);
  }

}
