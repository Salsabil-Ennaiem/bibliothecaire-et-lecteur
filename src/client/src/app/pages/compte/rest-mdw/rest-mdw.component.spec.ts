import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RestMdwComponent } from './rest-mdw.component';

describe('RestMdwComponent', () => {
  let component: RestMdwComponent;
  let fixture: ComponentFixture<RestMdwComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RestMdwComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RestMdwComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
