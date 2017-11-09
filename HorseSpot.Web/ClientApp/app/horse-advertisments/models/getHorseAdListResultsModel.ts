import { HorseAdListModel } from './horseAdListModel';

export interface GetHorseAdListResultsModel {
    HorseAdList: HorseAdListModel[];
    TotalCount: number;
}