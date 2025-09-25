import { ChartModule } from 'primeng/chart';
import { Subscription, timer } from 'rxjs';
import { Chart } from 'chart.js';
import { BookLoanCountDto, BookRotationRateDto, DashboardResponse, MonthlyLoanDto, MonthlyLossDto, MonthlyPolicyComparisonDto, UnusedBookDto, UserDelayCountDto } from '../../../model/dashbord.model';
import {Component, OnDestroy, OnInit } from '@angular/core';
import { DashboardService } from '../../../Services/dashboard.service';
import { TabViewModule } from 'primeng/tabview';


@Component({
  selector: 'app-tableaux-de-bord',
  imports: [ChartModule ,TabViewModule],
  templateUrl: './tableaux-de-bord.component.html',
  styleUrl: './tableaux-de-bord.component.css'
})


export class TableauxDeBordComponent implements OnInit, OnDestroy {
  activePartIndex: number = 0

  private subscription?: Subscription;
  dashboardData?: any;
  private topBooksLoansChart?: Chart;
  private bookRotationRatesChart?: Chart;
  private unusedBooksChart?: Chart;
  private delayRateChart?: Chart;
  private topUsersDelayChart?: Chart;
  private sanctionRateChart?: Chart;
  private monthlyLossesChart?: Chart;
  private delayVsLossChart?: Chart;
  private monthlyLoansChart?: Chart;
  private policyComparisonChart?: Chart;

  constructor(private dashService: DashboardService) { }

  ngOnInit() {
    this.subscription = timer(0, 60000).subscribe(() => this.loadDashboardData());
  }

  ngOnDestroy() {
    this.subscription?.unsubscribe();
    this.destroyAllCharts();
  }

  private destroyAllCharts() {
    this.topBooksLoansChart?.destroy();
    this.bookRotationRatesChart?.destroy();
    this.unusedBooksChart?.destroy();
    this.delayRateChart?.destroy();
    this.topUsersDelayChart?.destroy();
    this.sanctionRateChart?.destroy();
    this.monthlyLossesChart?.destroy();
    this.delayVsLossChart?.destroy();
    this.monthlyLoansChart?.destroy();
    this.policyComparisonChart?.destroy();
  }

  loadDashboardData() {
    this.dashService.getDashboardData('').subscribe({
      next: (data) => {
        this.dashboardData = data;
        this.renderTopBooksLoansChart();
        this.renderBookRotationRatesChart();
        this.renderUnusedBooksChart();
        this.renderDelayRateAndTopUsersCharts();
        this.renderSanctionAndMonthlyLossesChart();
        this.renderDelayVsLossChart();
        this.renderMonthlyLoansChart();
        this.renderPolicyComparisonChart();
      },
      error: (err) => console.error('Failed to load dashboard', err)
    });
  }

  private renderTopBooksLoansChart() {
    if (!this.dashboardData) return;
    const ctx = (document.getElementById('topBooksLoansChart') as HTMLCanvasElement).getContext('2d');
    this.topBooksLoansChart?.destroy();

    const top10Books = this.dashboardData.catalogueOptimization.topBooksLoans.slice(0, 10);
    this.topBooksLoansChart = new Chart(ctx!, {
      type: 'bar',
      data: {
        labels: top10Books.map((b: BookLoanCountDto) => b.bookTitle),
        datasets: [{
          label: 'Loan Count',
          data: top10Books.map((b: BookLoanCountDto) => b.loanCount),
          backgroundColor: top10Books.map(() =>
            `rgba(${Math.floor(Math.random() * 255)}, ${Math.floor(Math.random() * 255)}, ${Math.floor(Math.random() * 255)}, 0.7)`
          ),
        }]
      },
      options: {
        responsive: true,
        scales: {
          y: {
            beginAtZero: true,
            title: { display: true, text: 'Loan Count' }
          },
          x: {
            title: { display: true, text: 'Book Title' }
          }
        },
        plugins: {
          legend: { display: false },
          title: { display: true, text: 'Top 10 Books by Loan Count' }
        }
      }
    });
  }

  private renderBookRotationRatesChart() {
    if (!this.dashboardData) return;
    const ctx = (document.getElementById('bookRotationRatesChart') as HTMLCanvasElement).getContext('2d');
    this.bookRotationRatesChart?.destroy();

    const data = this.dashboardData.catalogueOptimization.bookRotationRates;
    this.bookRotationRatesChart = new Chart(ctx!, {
      type: 'bar',
      data: {
        labels: data.map((b: BookRotationRateDto) => b.bookTitle),
        datasets: [{
          label: 'Rotation Rate',
          data: data.map((b: BookRotationRateDto) => b.rotationRate),
          backgroundColor: 'rgba(75,192,192,0.7)'
        }]
      },
      options: {
        responsive: true,
        scales: {
          y: {
            beginAtZero: true,
            title: { display: true, text: 'Rotation Rate' }
          },
          x: {
            title: { display: true, text: 'Book Title' }
          }
        },
        plugins: {
          title: { display: true, text: 'Book Rotation Rates' }
        }
      }
    });
  }

  private renderUnusedBooksChart() {
    if (!this.dashboardData) return;
    const ctx = (document.getElementById('unusedBooksChart') as HTMLCanvasElement).getContext('2d');
    this.unusedBooksChart?.destroy();

    const data = this.dashboardData.catalogueOptimization.unusedBooks;
    this.unusedBooksChart = new Chart(ctx!, {
      type: 'bar',
      data: {
        labels: data.map((b: UnusedBookDto) => b.bookTitle),
        datasets: [{
          label: 'Months Since Last Loan',
          data: data.map((b: UnusedBookDto) => {
            const lastLoanDate = new Date(b.lastLoan);
            const diffMonths = (new Date().getTime() - lastLoanDate.getTime()) / (1000 * 3600 * 24 * 30);
            return diffMonths;
          }),
          backgroundColor: 'rgba(255, 159, 64, 0.7)'
        }]
      },
      options: {
        responsive: true,
        scales: {
          y: {
            beginAtZero: true,
            title: { display: true, text: 'Months Since Last Loan' }
          },
          x: {
            title: { display: true, text: 'Book Title' }
          }
        },
        plugins: {
          title: { display: true, text: 'Unused Books (Months Since Last Loan)' }
        }
      }
    });
  }

  private renderDelayRateAndTopUsersCharts() {
    if (!this.dashboardData) return;

    {
      const ctx = (document.getElementById('delayRateChart') as HTMLCanvasElement).getContext('2d');
      this.delayRateChart?.destroy();

      this.delayRateChart = new Chart(ctx!, {
        type: 'doughnut',
        data: {
          labels: ['Delay Rate', 'Other'],
          datasets: [{
            data: [this.dashboardData.delayReduction.delayRate, 100 - this.dashboardData.delayReduction.delayRate],
            backgroundColor: ['rgba(255, 99, 132, 0.7)', 'rgba(201, 203, 207, 0.7)']
          }]
        },
        options: {
          responsive: true,
          plugins: {
            title: { display: true, text: 'Delay Rate' }
          }
        }
      });
    }

    {
      const ctx = (document.getElementById('topUsersDelayChart') as HTMLCanvasElement).getContext('2d');
      this.topUsersDelayChart?.destroy();

      const topUsers = this.dashboardData.delayReduction.topDelayedUsers.slice(0, 5);
      this.topUsersDelayChart = new Chart(ctx!, {
        type: 'bar',
        data: {
          labels: topUsers.map((u: UserDelayCountDto) => u.userName),
          datasets: [{
            label: 'Delay Count',
            data: topUsers.map((u: UserDelayCountDto) => u.delayCount),
            backgroundColor: 'rgba(54, 162, 235, 0.7)'
          }]
        },
        options: {
          responsive: true,
          scales: {
            y: {
              beginAtZero: true,
              title: { display: true, text: 'Delay Count' }
            },
            x: {
              title: { display: true, text: 'User Name' }
            }
          },
          plugins: {
            title: { display: true, text: 'Top Users With Delays' }
          }
        }
      });
    }
  }

  private renderSanctionAndMonthlyLossesChart() {
    if (!this.dashboardData) return;

    {
      const ctx = (document.getElementById('sanctionRateChart') as HTMLCanvasElement).getContext('2d');
      this.sanctionRateChart?.destroy();

      this.sanctionRateChart = new Chart(ctx!, {
        type: 'doughnut',
        data: {
          labels: ['Sanction Rate', 'Other'],
          datasets: [{
            data: [this.dashboardData.lossAnalysis.sanctionRate, 100 - this.dashboardData.lossAnalysis.sanctionRate],
            backgroundColor: ['rgba(255, 206, 86, 0.7)', 'rgba(201, 203, 207, 0.7)']
          }]
        },
        options: {
          responsive: true,
          plugins: {
            title: { display: true, text: 'Sanction Rate' }
          }
        }
      });
    }

    {
      const ctx = (document.getElementById('monthlyLossesChart') as HTMLCanvasElement).getContext('2d');
      this.monthlyLossesChart?.destroy();

      const data = this.dashboardData.lossAnalysis.monthlyLosses;
      this.monthlyLossesChart = new Chart(ctx!, {
        type: 'bar',
        data: {
          labels: data.map((d: MonthlyLossDto) => `${d.month}/${d.year}`),
          datasets: [
            {
              type: 'bar',
              label: 'Loss Cost',
              data: data.map((d: MonthlyLossDto) => d.lossCost),
              backgroundColor: 'rgba(255, 159, 64, 0.7)'
            },
            {
              type: 'line',
              label: 'Fine Amount',
              data: data.map((d: MonthlyLossDto) => d.fineAmount),
              borderColor: 'rgba(255, 99, 132, 0.7)',
              fill: false,
              tension: 0.3,
              yAxisID: 'y1'
            }
          ]
        },
        options: {
          responsive: true,
          scales: {
            y: {
              type: 'linear',
              position: 'left',
              title: { display: true, text: 'Loss Cost (USD)' }
            },
            y1: {
              type: 'linear',
              position: 'right',
              title: { display: true, text: 'Fine Amount (USD)' },
              grid: { drawOnChartArea: false }
            },
            x: {
              title: { display: true, text: 'Month/Year' }
            }
          },
          plugins: {
            title: { display: true, text: 'Monthly Losses and Fines' }
          }
        }
      });
    }
  }

  private renderDelayVsLossChart() {
    if (!this.dashboardData) return;
    const ctx = (document.getElementById('delayVsLossChart') as HTMLCanvasElement).getContext('2d');
    this.delayVsLossChart?.destroy();

    const data = this.dashboardData.lossAnalysis.delayVsLoss;

    this.delayVsLossChart = new Chart(ctx!, {
      type: 'bar',
      data: {
        labels: ['Delay Count', 'Loss Count'],
        datasets: [{
          label: 'Count',
          data: [data.delayCount, data.lossCount],
          backgroundColor: ['rgba(153, 102, 255, 0.7)', 'rgba(255, 159, 64, 0.7)']
        }]
      },
      options: {
        responsive: true,
        scales: {
          y: {
            beginAtZero: true,
            title: { display: true, text: 'Counts' }
          },
          x: {
            title: { display: true, text: 'Metric Type' }
          }
        },
        plugins: {
          title: { display: true, text: 'Delay vs Loss Counts' }
        }
      }
    });
  }

  private renderMonthlyLoansChart() {
    if (!this.dashboardData) return;
    const ctx = (document.getElementById('monthlyLoansChart') as HTMLCanvasElement).getContext('2d');
    this.monthlyLoansChart?.destroy();

    const data = this.dashboardData.resourcePlanning.monthlyLoans;
    this.monthlyLoansChart = new Chart(ctx!, {
      type: 'line',
      data: {
        labels: data.map((d: MonthlyLoanDto) => `${d.month}/${d.year}`),
        datasets: [{
          label: 'Monthly Loans',
          data: data.map((b: MonthlyLoanDto) => b.loanCount),
          borderColor: 'rgba(54, 162, 235, 0.7)',
          fill: false,
          tension: 0.3
        }]
      },
      options: {
        responsive: true,
        scales: {
          y: {
            beginAtZero: true,
            title: { display: true, text: 'Loan Count' }
          },
          x: {
            title: { display: true, text: 'Month/Year' }
          }
        },
        plugins: {
          title: { display: true, text: 'Monthly Loans Over Time' }
        }
      }
    });
  }

  private renderPolicyComparisonChart() {
    if (!this.dashboardData) return;
    const ctx = (document.getElementById('policyComparisonChart') as HTMLCanvasElement).getContext('2d');
    this.policyComparisonChart?.destroy();

    const data = this.dashboardData.policyEvaluation.monthlyComparison;
    this.policyComparisonChart = new Chart(ctx!, {
      type: 'line',
      data: {
        labels: data.map((d: MonthlyPolicyComparisonDto) => `${d.month}/${d.year}`),
        datasets: [
          {
            label: 'Before Policy',
            data: data.map((d: MonthlyPolicyComparisonDto) => d.beforeRate),
            borderColor: 'rgba(255, 99, 132, 0.7)',
            fill: false,
            tension: 0.3
          },
          {
            label: 'After Policy',
            data: data.map((d: MonthlyPolicyComparisonDto) => d.afterRate),
            borderColor: 'rgba(54, 162, 235, 0.7)',
            fill: false,
            tension: 0.3
          }
        ]
      },
      options: {
        responsive: true,
        scales: {
          y: {
            type: 'linear',
            position: 'left',
            title: { display: true, text: 'Before Rate (%)' }
          },
          y1: {
            type: 'linear',
            position: 'right',
            title: { display: true, text: 'After Rate (%)' },
            grid: { drawOnChartArea: false }
          },
          x: {
            title: { display: true, text: 'Month/Year' }
          }
        },
        plugins: {
          title: { display: true, text: 'Policy Delay Rate Comparison' }
        }
      }
    });
  }
}