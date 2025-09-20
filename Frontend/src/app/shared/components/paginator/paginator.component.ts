import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-paginator',
  standalone: true,
  templateUrl: './paginator.component.html',
})
export class PaginatorComponent {
  @Input() page = 1;
  @Input() size = 20;
  @Input() total = 0;

  @Output() pageChange = new EventEmitter<number>();

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.total / this.size));
  }

  prev() {
    if (this.page > 1) this.pageChange.emit(this.page - 1);
  }

  next() {
    if (this.page < this.totalPages) this.pageChange.emit(this.page + 1);
  }
}
