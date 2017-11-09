import { Injectable } from '@angular/core';
import { Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

@Injectable()
export class StorageService {
    constructor(@Inject(PLATFORM_ID) private platformId: Object) { }

    getItem(key: string) {
        if (isPlatformBrowser(this.platformId)) {
            return localStorage.getItem(key);
        }

        return null;
    }
}