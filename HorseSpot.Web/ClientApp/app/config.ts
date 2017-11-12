import { environment } from '../environments/environment';

export let CONFIG = {
    baseUrls: {
        apiUrl: 'http://localhost/horsespotservices/api/',
    },

    imagesUrl: 'http://localhost/horsespotservices',
    
    authUrl: 'http://localhost/horsespotservices/token',

    restCountriesUrl: 'https://restcountries.eu/rest/v2/name/',
    
    webSocketUrl: 'ws://localhost/horsespotservices' + '/api/appointment?userId=',

    accepted_file_extension: [ '.JPEG', '.JPG', '.PNG', '.BMP', '.TIFF'],

    adminRole: "Admin",

    client_id: "Horsespot_Web_Angular2",

    login_grant_type: "password",

    refresh_token_grant_type: "refresh_token",

    mobile_width: 767,

    _true: "true",

    _false: "false",

    allImagesSizeLimit: 5242880, //bytes

    horseSpot_contact_email: "contact@horse-spot.com",
    
    genders: [ "Gelding", "Mare", "Stallion" ],

    languages: [ { value: "en", displayText: "English", imgUrl: "../../assets/images/english.png" }, 
                 { value: "de", displayText: "Deutsch", imgUrl: "../../assets/images/germany.png" },
                 { value: "fr", displayText: "French", imgUrl: "../../assets/images/france.png" },
                 { value: "ro", displayText: "Romanian", imgUrl: "../../assets/images/romania.png" }],

    defaultAge: { min: 4, max: 13 },
    defaultPrice: { min: 10000, max: 30000 },
    defaultHeight: { min: 160, max: 178 } 
}
 
export * from './config';
