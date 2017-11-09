export class SearchModel {
    PageNumber: number;
    Gender: string;
    AgeModel: BetweenAge;
    HeightModel: BetweenHeight;
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
        this.AgeModel = <BetweenAge> { };
        this.HeightModel = <BetweenHeight> { };
        this.PriceRangeId = 0;
        this.GenderId = 0;
        this.SuitableFor = new Array<number>();
        this.SortModel = new SortModel();
    }
}

export class BetweenAge {
    MinAge: number;
    MaxAge: number;
}

export class BetweenHeight {
    MinHeight: number;
    MaxHeight: number;
}

export class SortModel {
    SortAfter: string;
    SortDirection: number;

    constructor() {
        this.SortAfter = "Date Posted";
        this.SortDirection = 1;
    }
}
