import { Component, OnInit } from '@angular/core';
import { MembreService } from '../../../../Services/membre.service';
import { CommonModule } from '@angular/common';
import { MembreDto, StatutMemb, TypeMemb } from '../../../../model/membre.model';
import { CardModule } from 'primeng/card';


@Component({
  selector: 'app-list-membre',
  imports: [CommonModule, CardModule],
  templateUrl: './list-membre.component.html',
  styleUrl: './list-membre.component.css'
})
export class ListMembreComponent implements OnInit {

  constructor(private MemServ: MembreService) { }
  users: MembreDto[] = []
  ngOnInit(): void {
    this.loadMembres();
  }

  loadMembres(): void {
    this.MemServ.getAll().subscribe({
      next: (data) => this.users = data,
      error: (err) => console.error('Erreur chargement Emp', err)
    });
  }
  public Getvalue(value: number) {
    return StatutMemb[value];
  }
  public IconTypeMem(value: number) {
    if (value == 0)
      return "👨‍🏫";
    else if (value == 1)
      return "🎓";
    else
      return "📚";
  }
}
