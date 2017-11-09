import { environment } from '../environments/environment';

export let CONFIG = {
    baseUrls: {
        apiUrl: 'http://localhost/horsespotservices/api/',
    },

    imagesUrl: 'http://localhost/horsespotservices',
    
    authUrl: 'http://localhost/horsespotservices/token',
    
    webSocketUrl: 'ws://localhost/horsespotservices' + '/api/appointment?userId=',

    accepted_file_extension: [ '.JPEG', '.JPG', '.PNG', '.BMP', '.TIFF'],

    adminRole: "Admin",

    client_id: "Horsespot_Web_Angular2",

    login_grant_type: "password",

    refresh_token_grant_type: "refresh_token",

    limitFilesUpload: 6,

    allImagesSizeLimit: 5242880, //bytes

    horseSpot_contact_email: "contact@horse-spot.com",
    
    genders: [ "Gelding", "Mare", "Stallion" ],

    languages: [ { value: "en", displayText: "English", imgUrl: "../../assets/images/english.png" }, 
                 { value: "de", displayText: "Deutsch", imgUrl: "../../wwwroot/assets/images/germany.png" },
                 { value: "fr", displayText: "French", imgUrl: "../../wwwroot/assets/images/france.png" },
                 { value: "ro", displayText: "Romanian", imgUrl: "../../wwwroot/assets/images/romania.png" }]
}
 
export * from './config';
