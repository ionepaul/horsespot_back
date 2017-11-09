import { HorseAbilityModel } from './horseAbilityModel';
import { RecommendedRiderModel } from './recommendedRiderModel';
import { PriceRangeModel } from './priceRangeModel';
import { AddressModel } from './addressModel';
import { GenderModel } from './genderModel';
import { PedigreeModel } from './pedigree';
import { ImageModel } from './imageModel';

export interface HorseAdModel {
    Id: number;
    UserId: string;
    Title: string;
    HorseName: string;
    Gender: string;
    Age: number;
    Breed: string;
    HeightInCm: number;
    Description: string;
    Abilities: Array<HorseAbilityModel>;
    VideoLink: string;
    Pedigree: PedigreeModel;
    HaveXRays: boolean;
    Address: AddressModel;
    HaveCompetionalExperience: boolean;
    RecomendedRiders: Array<RecommendedRiderModel>;
    Price: number;
    PriceRange: PriceRangeModel;
    PriceRangeId: number;
    Images: Array<ImageModel>;
    IsValidated: boolean;
    DatePosted: Date;
    CountFavoritesFor: number;
    Views: number;
    FavoritesFor: Array<string>;
    RecomendedRidersIds: Array<number>;
    AbilityIds: Array<number>;
}