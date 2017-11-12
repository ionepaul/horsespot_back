import { CONFIG } from '../../config';

export class SearchModel {
  PageNumber: number;
  Gender: string;
  AgeModel: BetweenAge;
  HeightModel: BetweenHeight;
  PriceModel: BetweenPrice;
  Breed: string;
  AbilityId: number;
  ToHaveXRays: boolean;
  ToHaveCompetionalExperience: boolean;
  SuitableFor: Array<number>;
  PriceRangeId: number;
  AfterFatherName: string;
  Country: string;
  SortModel: SortModel;
  GenderId: number;

  constructor() {
    this.AgeModel = new BetweenAge(CONFIG.defaultAge.min, CONFIG.defaultAge.max);
    this.HeightModel = new BetweenHeight(CONFIG.defaultHeight.min, CONFIG.defaultHeight.max);
    this.PriceModel = new BetweenPrice(CONFIG.defaultPrice.min, CONFIG.defaultPrice.max);
    this.PriceRangeId = 0;
    this.GenderId = 0;
    this.SuitableFor = new Array<number>();
    this.SortModel = new SortModel();
  }
}

export class BetweenAge {
  MinAge: number;
  MaxAge: number;

  constructor(minAge: number, maxAge: number) {
    this.MinAge = minAge;
    this.MaxAge = maxAge;
  }
}

export class BetweenHeight {
  MinHeight: number;
  MaxHeight: number;

  constructor(minHeight: number, maxHeight: number) {
    this.MinHeight = minHeight;
    this.MaxHeight = maxHeight;
  }
}

export class BetweenPrice {
  MinPrice: number;
  MaxPrice: number;

  constructor(minPrice: number, maxPrice: number) {
    this.MinPrice = minPrice;
    this.MaxPrice = maxPrice;
  }
}

export class SortModel {
  SortAfter: string;
  SortDirection: number;

  constructor() {
    this.SortAfter = "Date Posted";
    this.SortDirection = 1;
  }
}
