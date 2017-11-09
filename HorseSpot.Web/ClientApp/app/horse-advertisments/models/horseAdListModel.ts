import { PriceRangeModel } from './priceRangeModel';

export interface HorseAdListModel {
    Id: string;
    Title: string;
    HorseName: string;
    Age: number;
    Breed: string;
    Price: number;
    Country: string;
    PriceRange: PriceRangeModel;
    ImageId: string;
    IsValidated: boolean;
    CountFavoritesFor: number;
    Views: number;
    DatePosted: Date;
    Description: string;
}