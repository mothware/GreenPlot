export type PlantCategory = 'Vegetable' | 'Herb' | 'Fruit' | 'Flower' | 'CoverCrop'
export type PlantLifecycle = 'Annual' | 'Biennial' | 'Perennial'
export type SunRequirement = 'FullSun' | 'PartialSun' | 'Shade'
export type BedType = 'Raised' | 'InGround' | 'Container' | 'Row'
export type BedShape = 'Rectangle' | 'Square' | 'LShape' | 'Circle' | 'CustomPolygon'
export type PlantingMethod = 'DirectSow' | 'Transplant'
export type PlantingState = 'Planned' | 'Sown' | 'Germinated' | 'PottedUp' | 'HardenedOff' | 'Transplanted' | 'Flowering' | 'Harvested' | 'Ended'
export type ActivityType = 'Sown' | 'Germinated' | 'PottedUp' | 'HardenedOff' | 'Transplanted' | 'Flowering' | 'Harvested' | 'Ended' | 'Observation' | 'Watered' | 'Fertilized' | 'Pruned' | 'Treated'
export type CompanionEffect = 'Good' | 'Neutral' | 'Bad'

export interface Plant {
  id: string
  commonName: string
  scientificName: string
  family: string
  category: PlantCategory
  lifecycle: PlantLifecycle
  sunRequirement: SunRequirement
  waterNeeds: string
  daysToGerminateMin?: number
  daysToGerminateMax?: number
  daysToMaturityMin?: number
  daysToMaturityMax?: number
  sowingDepthInches?: number
  spacingInches?: number
  hardinessZoneMin?: string
  hardinessZoneMax?: string
  notes?: string
  isGlobal: boolean
  varietyCount: number
}

export interface Variety {
  id: string
  plantId: string
  plantCommonName: string
  name: string
  daysToMaturity?: number
  daysToGerminate?: number
  sowingDepthInches?: number
  spacingInches?: number
  sunRequirement?: SunRequirement
  diseaseResistance?: string
  flavorNotes?: string
  colorNotes?: string
  source?: string
  notes?: string
  isGlobal: boolean
}

export interface Garden {
  id: string
  name: string
  zipCode?: string
  hardinessZone?: string
  lastFrostDate?: string
  firstFrostDate?: string
  notes?: string
  isArchived: boolean
  bedCount: number
  createdAt: string
}

export interface Bed {
  id: string
  gardenId: string
  name: string
  type: BedType
  shape: BedShape
  widthFeet: number
  lengthFeet: number
  gridCellSizeFeet: number
  sunExposure?: SunRequirement
  isArchived: boolean
  plantingCount: number
}

export interface BedCell {
  id: string
  x: number
  y: number
  planting?: PlantingSummary
}

export interface BedDetail extends Bed {
  soilNotes?: string
  cells: BedCell[]
  plantings: PlantingSummary[]
}

export interface PlantingSummary {
  id: string
  varietyId: string
  varietyName: string
  plantCommonName: string
  plantFamily: string
  state: PlantingState
  method: PlantingMethod
  startDate: string
}

export interface Planting extends PlantingSummary {
  ownerId: string
  bedId: string
  seedLotId?: string
  seasonId: string
  seasonName: string
  notes?: string
  companionWarningOverridden: boolean
  activities: Activity[]
  companionWarnings: CompanionWarning[]
}

export interface Activity {
  id: string
  plantingId: string
  type: ActivityType
  occurredAt: string
  quantity?: number
  weightGrams?: number
  notes?: string
  photoUrls: string[]
}

export interface CompanionWarning {
  adjacentPlantingId: string
  adjacentVarietyName: string
  adjacentPlantName: string
  effect: CompanionEffect
  reasoning?: string
}

export interface CalendarEvent {
  id: string
  title: string
  startDate: string
  endDate?: string
  eventType: string
  plantingId?: string
  gardenId?: string
  bedId?: string
  varietyName?: string
  plantName?: string
  gardenName?: string
  bedName?: string
}

export interface PagedResult<T> {
  items: T[]
  totalCount: number
  page: number
  pageSize: number
}
