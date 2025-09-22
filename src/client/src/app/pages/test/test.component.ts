import {  Component, OnDestroy, OnInit} from '@angular/core';
import { ChartModule } from 'primeng/chart';
import { Subscription } from 'rxjs';
import { DashboardSignalRService } from '../../Services/Signalr/dashboard-signalr.service';
import { DashboardResponse } from '../../model/dashbord.model';
import { DashboardService } from '../../Services/dashboard.service';


@Component({
  selector: 'app-test',
  imports: [ChartModule ],
  templateUrl: './test.component.html',
  styleUrl: './test.component.css'
})

export class TestComponent  implements OnInit, OnDestroy {
  private subscription: Subscription | undefined;

  constructor(private signalrService: DashboardSignalRService ,private dashboardService: DashboardService) {}
  dashboardData!: DashboardResponse;

  topBooksLoansData: any;
  delayReductionData: any;
  lossAnalysisData: any;
  resourcePlanningData: any;

  chartOptions = {
    responsive: true,
    maintainAspectRatio: false,
  };


  ngOnInit(): void {
    this.loadDashboard('exampleBiblioId');
  }

  loadDashboard(biblioId: string) {
    this.dashboardService.getDashboardData(biblioId).subscribe({
      next: data => {
        this.dashboardData = data;
        this.prepareChartData();
      },
      error: err => console.error('Error loading dashboard:', err),
    });
  }

  prepareChartData() {
    // Prepare Bar chart dataset for Top Books Loans
    this.topBooksLoansData = {
      labels: this.dashboardData.catalogueOptimization.topBooksLoans.map(b => b.bookTitle),
      datasets: [
        {
          label: 'Loan Count',
          backgroundColor: '#42A5F5',
          data: this.dashboardData.catalogueOptimization.topBooksLoans.map(b => b.loanCount),
        },
      ],
    };

    // Prepare Doughnut chart for Delay Rate
    this.delayReductionData = {
      labels: ['Delay Rate', 'On-time Rate'],
      datasets: [
        {
          data: [this.dashboardData.delayReduction.delayRate, 100 - this.dashboardData.delayReduction.delayRate],
          backgroundColor: ['#FF6384', '#36A2EB'],
        },
      ],
    };

    // Prepare Line chart for Monthly Losses
    this.lossAnalysisData = {
      labels: this.dashboardData.lossAnalysis.monthlyLosses.map(m => `${m.month}/${m.year}`),
      datasets: [
        {
          label: 'Loss Cost',
          fill: false,
          borderColor: '#F44336',
          data: this.dashboardData.lossAnalysis.monthlyLosses.map(m => m.lossCost),
        },
        {
          label: 'Fine Amount',
          fill: false,
          borderColor: '#2196F3',
          data: this.dashboardData.lossAnalysis.monthlyLosses.map(m => m.fineAmount),
        },
      ],
    };

    // Prepare Bar chart for Monthly Loans
    this.resourcePlanningData = {
      labels: this.dashboardData.resourcePlanning.monthlyLoans.map(m => `${m.month}/${m.year}`),
      datasets: [
        {
          label: 'Loan Count',
          backgroundColor: '#66BB6A',
          data: this.dashboardData.resourcePlanning.monthlyLoans.map(m => m.loanCount),
        },
      ],
    };
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
    this.signalrService.disconnect('exampleBiblioId');
  }
}