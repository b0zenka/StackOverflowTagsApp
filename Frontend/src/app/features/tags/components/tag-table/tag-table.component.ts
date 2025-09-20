import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Tag } from '../../../../core/models/tag.model';
import { CommonModule } from '@angular/common';

type SortField = 'name' | 'count' | 'share';

type SortOrder = 'asc' | 'desc';

@Component({
  selector: 'app-tag-table',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './tag-table.component.html',
})
export class TagTableComponent {
  @Input() rows: Tag[] = [];
  @Input() sortBy: SortField = 'name';
  @Input() order: SortOrder = 'asc';

  @Output() sortChange = new EventEmitter<{
    sortBy: SortField;
    order: SortOrder;
  }>();

  changeSort(field: SortField) {
    const order: SortOrder =
      this.sortBy === field ? (this.order === 'asc' ? 'desc' : 'asc') : 'asc';
    this.sortChange.emit({ sortBy: field, order });
  }
}
