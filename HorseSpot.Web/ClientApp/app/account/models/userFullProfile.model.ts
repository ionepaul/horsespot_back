import { HorseAdListModel } from '../../horse-advertisments/models/horseAdListModel';

export class UserFullProfile {
    UserId: string;
    FullName: string;
    PhoneNumber: string;
    Email: string;
    TotalForSale: number;
    TotalReferenes: number;
    TotalFavorites: number;
    TotalMeetings: number;
    TotalFeedback: number;
    HorsesForSale: Array<HorseAdListModel>
    FavoriteHorses: Array<HorseAdListModel>
    ReferenceHorses: Array<HorseAdListModel>
}