import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';  // <-- potrzebne
import { TagService } from '../../../../core/services/tag.service';
import { Tag } from '../../../../core/models/tag.model';

// Importy child komponentów
import { TagTableComponent } from '../../components/tag-table/tag-table.component';
import { PaginatorComponent } from '../../../../shared/components/paginator/paginator.component';
import { LoadingSpinnerComponent } from '../../../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-tag-list-page',
  standalone: true,
  imports: [
    CommonModule,            // <-- tutaj
    TagTableComponent,
    PaginatorComponent,
    LoadingSpinnerComponent
  ],
  templateUrl: './tag-list-page.component.html'
})
export class TagListPageComponent implements OnInit {
  rows: Tag[] = [];
  page = 1;
  size = 20;
  total = 0;
  sortBy: 'name'|'count'|'share' = 'name';
  order: 'asc'|'desc' = 'asc';
  loading = false;

  constructor(private api: TagService) {}

  ngOnInit(): void {
    this.load();
  }

  load() {
    this.loading = true;
    this.api.getTags(this.page, this.size, this.sortBy, this.order).subscribe({
      next: r => { this.rows = r.data; this.total = r.total; this.loading = false; },
      error: _ => this.loading = false
    });
  }

  onSortChange(e: { sortBy: 'name'|'count'|'share'; order: 'asc'|'desc' }) {
    this.sortBy = e.sortBy;
    this.order = e.order;
    this.page = 1;
    this.load();
  }

  onPageChange(p: number) {
    this.page = p;
    this.load();
  }

  refresh() {
    this.loading = true;
    this.api.refreshTags().subscribe({
      next: () => this.load(),
      error: _ => this.loading = false
    });
  }
}
