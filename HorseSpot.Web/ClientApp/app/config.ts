import { environment } from '../environments/environment';

export let CONFIG = {
  baseUrls: {
    apiUrl: environment.httpBaseUrl + environment.virtualAppName  + '/api/',
  },

  imagesUrl: environment.httpBaseUrl + environment.virtualAppName,

  horseAdsImagesUrl: environment.httpBaseUrl + environment.virtualAppName + '/Images/HorseAdsImg/',

  profileImagesUrl: environment.httpBaseUrl + environment.virtualAppName + '/Images/ProfilePhotos/',

  authUrl: environment.httpBaseUrl + environment.virtualAppName + 'api.horsespot/token',

  restCountriesUrl: 'https://restcountries.eu/rest/v2/name/',

  webSocketUrl: 'ws://192.168.100.7/horsespotservices' + '/api/appointment?userId=',

  accepted_file_extension: ['.JPEG', '.JPG', '.PNG', '.BMP', '.TIFF'],

  adminRole: "Admin",

  client_id: "Horsespot_Web_Angular2",

  login_grant_type: "password",

  refresh_token_grant_type: "refresh_token",

  refresh_token_lifetime_min: 7200,

  mobile_width: 767,

  _true: "true",

  _false: "false",

  allImagesSizeLimit: 5242880, //bytes

  horseSpot_contact_email: "contact@horse-spot.com",

  genders: ["Gelding", "Mare", "Stallion"],

  gender: "Gender",

  languages: [{ value: "en", displayText: "English", imgUrl: "../../assets/images/gb.svg" },
  { value: "de", displayText: "Deutsche", imgUrl: "../../assets/images/de.svg" },
  { value: "fr", displayText: "Française", imgUrl: "../../assets/images/fr.svg" },
  { value: "ro", displayText: "Română", imgUrl: "../../assets/images/ro.svg" }],

  defaultAge: { min: 4, max: 13 },
  defaultPrice: { min: 10000, max: 30000 },
  defaultHeight: { min: 160, max: 178 },

  dbMaxPriceRangeValue: "40,000 +",
  frontMaxPriceRangeValue: 100000,
  adsPerPage: 12,

  fbAppId: '275509216289907',
  fbSdkVersion: 'v2.8'
}

export * from './config';
