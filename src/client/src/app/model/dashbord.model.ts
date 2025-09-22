export interface DashboardResponse {
    catalogueOptimization: CatalogueOptimizationDto;
    delayReduction: DelayReductionDto;
    lossAnalysis: LossAnalysisDto;
    resourcePlanning: ResourcePlanningDto;
    policyEvaluation: PolicyEvaluationDto;
}

export interface CatalogueOptimizationDto {
    topBooksLoans: BookLoanCountDto[];
    bookRotationRates: BookRotationRateDto[];
    unusedBooks: UnusedBookDto[];
}

export interface DelayReductionDto {
    delayRate: number;
    topDelayedUsers: UserDelayCountDto[];
    problematicBooks: BookDelayCountDto[];
}

export interface LossAnalysisDto {
    sanctionRate: number;
    monthlyLosses: MonthlyLossDto[];
    delayVsLoss: DelayVsLossDto;
    totalLossCost: number;
}

export interface ResourcePlanningDto {
    monthlyLoans: MonthlyLoanDto[];
    averageLoanDuration: number;
}

export interface PolicyEvaluationDto {
    delayRateBeforePolicy: number;
    delayRateAfterPolicy: number;
    monthlyComparison: MonthlyPolicyComparisonDto[];
}

export interface BookLoanCountDto {
    bookTitle: string;
    loanCount: number;
    bookId: string;
}

export interface BookRotationRateDto {
    bookTitle: string;
    rotationRate: number;
    available: number;
    loaned: number;
}

export interface UnusedBookDto {
    bookTitle: string;
    lastLoan: string;
    bookId: string;
}

export interface UserDelayCountDto {
    userName: string;
    delayCount: number;
    userId: string;
}

export interface BookDelayCountDto {
    bookTitle: string;
    delayCount: number;
    bookId: string;
}

export interface MonthlyLossDto {
    month: number;
    year: number;
    lossCost: number;
    fineAmount: number;
}

export interface DelayVsLossDto {
    delayCount: number;
    lossCount: number;
}

export interface MonthlyLoanDto {
    month: number;
    year: number;
    loanCount: number;
}

export interface MonthlyPolicyComparisonDto {
    month: number;
    year: number;
    beforeRate: number;
    afterRate: number;
}