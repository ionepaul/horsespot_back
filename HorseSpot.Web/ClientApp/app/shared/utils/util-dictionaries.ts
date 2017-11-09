import { CONFIG } from '../../config';
import { Dictionary } from '../../shared/utils/dictionary';

export class UtilDictionaries { 
    categoryUrlDictionaryByName: Dictionary;
    categoryUrlDictionaryById: Dictionary;
    genderNameDictionaryById: Dictionary;
    
    constructor() {
        this.categoryUrlDictionaryByName = { 'showjumping': '1', 
                                             'dressage': '2',
                                             'eventing': '3',
                                             'endurance': '4',
                                             'driving': '5',
                                             'leisure': '6',
                                             'foals': '7' }
        
        this.categoryUrlDictionaryById = { '0': 'showjumping',
                                           '1': 'showjumping', 
                                           '2': 'dressage',
                                           '3': 'eventing',
                                           '4': 'endurance',
                                           '5': 'driving',
                                           '6': 'leisure',
                                           '7': 'foals' }

        this.genderNameDictionaryById = { '1': 'Gelding',
                                          '2': 'Mare',
                                          '3': 'Stallion'}
    }

    getUrlByCategoryName(categoryName: string) {
        return this.categoryUrlDictionaryByName[categoryName];
    }

    getUrlByCategoryId(categoryId: number) {
        return this.categoryUrlDictionaryById[categoryId];
    }

    getGenderNameById(genderId: number) {
        return this.genderNameDictionaryById[genderId];
    }
}