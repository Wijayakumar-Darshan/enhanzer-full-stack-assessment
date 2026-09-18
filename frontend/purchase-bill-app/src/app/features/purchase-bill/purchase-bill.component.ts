import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { LocationService } from '../../core/services/location.service';
import { PurchaseBillService } from '../../core/services/purchase-bill.service';
import { AuthService } from '../../core/services/auth.service';
import { Location, PurchaseBillItem } from '../../core/models/models';

const ITEM_OPTIONS = ['Mango', 'Apple', 'Banana', 'Orange', 'Grapes', 'Kiwi', 'Strawberry'];

@Component({
  selector: 'app-purchase-bill',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './purchase-bill.component.html',
  styleUrls: ['./purchase-bill.component.scss']
})
export class PurchaseBillComponent implements OnInit {
  private fb = inject(FormBuilder);
  private locationService = inject(LocationService);
  private purchaseBillService = inject(PurchaseBillService);
  private authService = inject(AuthService);
  private router = inject(Router);

  itemOptions = ITEM_OPTIONS;
  filteredItemOptions = ITEM_OPTIONS;

  locations: Location[] = [];
  items: PurchaseBillItem[] = [];

  isLoadingLocations = false;
  isSubmitting = false;
  errorMessage = '';

  form = this.fb.group({
    itemName: ['', Validators.required],
    batch: ['', Validators.required],
    standardCost: [0, [Validators.required, Validators.min(0)]],
    standardPrice: [0, [Validators.required, Validators.min(0)]],
    margin: [{ value: 0, disabled: true }],
    qty: [0, [Validators.required, Validators.min(1)]],
    freeQty: [0, [Validators.min(0)]],
    discount: [0, [Validators.min(0), Validators.max(100)]]
  });

  ngOnInit(): void {
    this.loadLocations();
    this.loadExistingItems();

    // Recalculate margin live as cost/price change.
    this.form.get('standardCost')!.valueChanges.subscribe(() => this.updateMargin());
    this.form.get('standardPrice')!.valueChanges.subscribe(() => this.updateMargin());
  }

  private loadLocations(): void {
    this.isLoadingLocations = true;
    this.locationService.getLocations().subscribe({
      next: (locations) => {
        this.locations = locations;
        this.isLoadingLocations = false;
      },
      error: () => {
        this.isLoadingLocations = false;
        this.errorMessage = 'Could not load locations for the Batch dropdown.';
      }
    });
  }

  private loadExistingItems(): void {
    this.purchaseBillService.getAll().subscribe({
      next: (items) => (this.items = items),
      error: () => {
        // Non-fatal: the form still works even if history can't be loaded.
      }
    });
  }

  filterItems(query: string): void {
    const q = query.toLowerCase();
    this.filteredItemOptions = this.itemOptions.filter((i) => i.toLowerCase().includes(q));
  }

  private updateMargin(): void {
    const cost = Number(this.form.get('standardCost')!.value) || 0;
    const price = Number(this.form.get('standardPrice')!.value) || 0;
    const margin = price - cost;
    this.form.get('margin')!.setValue(margin, { emitEvent: false });
  }

  get totalCostPreview(): number {
    const { standardCost, qty, discount } = this.form.getRawValue();
    const cost = Number(standardCost) || 0;
    const q = Number(qty) || 0;
    const disc = Number(discount) || 0;
    return cost * q * (1 - disc / 100);
  }

  get totalSellingPreview(): number {
    const { standardPrice, qty } = this.form.getRawValue();
    const price = Number(standardPrice) || 0;
    const q = Number(qty) || 0;
    return price * q;
  }

  onAdd(): void {
    this.errorMessage = '';

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();

    const newItem: PurchaseBillItem = {
      itemName: raw.itemName!,
      batch: raw.batch!,
      standardCost: Number(raw.standardCost),
      standardPrice: Number(raw.standardPrice),
      margin: Number(raw.margin),
      qty: Number(raw.qty),
      freeQty: Number(raw.freeQty) || 0,
      discount: Number(raw.discount) || 0,
      totalCost: this.totalCostPreview,
      totalSelling: this.totalSellingPreview
    };

    this.isSubmitting = true;
    this.purchaseBillService.add(newItem).subscribe({
      next: (saved) => {
        this.items = [saved, ...this.items];
        this.isSubmitting = false;
        this.resetForm();
      },
      error: (err) => {
        this.isSubmitting = false;
        this.errorMessage = err?.error?.message || 'Could not save the item. Please try again.';
      }
    });
  }

  private resetForm(): void {
    this.form.reset({
      itemName: '',
      batch: '',
      standardCost: 0,
      standardPrice: 0,
      margin: 0,
      qty: 0,
      freeQty: 0,
      discount: 0
    });
  }

  get totalItems(): number {
    return this.items.length;
  }

  get totalQty(): number {
    return this.items.reduce((sum, i) => sum + (Number(i.qty) || 0), 0);
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
