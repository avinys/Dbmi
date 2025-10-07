# Bdmi — Projekto dokumentacija

## 1. Sprendžiamo uždavinio aprašymas

### 1.1 Sistemos paskirtis

Projekto tikslas – sukurti paprastą, aiškią ir patogią **filmų vertinimo sistemą**. Vartotojai gali naršyti filmus ir jų aprašus, **reitinguoti** bei **rašyti atsiliepimus**, o kūrėjai – **įkelti savo filmus**. Sistema susideda iš dviejų dalių: **žiniatinklio aplikacijos (Front‑End)** ir **REST API (Back‑End)**, kuris perduoda duomenis tarp kliento ir serverio dalių.

Svečiai gali naršyti po filmų katalogą, filtruoti pagal **žanrą, trukmę, kalbą**, matyti reitingus bei atsiliepimus. Prisiregistravę naudotojai gali **pridėti naujus filmus**, rašyti atsiliepimus ir pateikti reitingus. **Administratorius** prižiūri sistemos veikimą, turi prieigą prie filmų, atsiliepimų ir naudotojų duomenų, gali **tvirtinti įkėlimus**, **trinti** ir **redaguoti atsiliepimus**.

### 1.2 Funkciniai reikalavimai

**Neregistruotas vartotojas** gali:

-   Peržiūrėti filmus ir vieno filmo puslapį su atsiliepimais
-   Susikurti paskyrą
-   Prisijungti prie sistemos

**Registruotas vartotojas** gali:

-   Prisijungti ir atsijungti
-   Peržiūrėti ir filtruoti filmų sąrašą
-   Peržiūrėti vieno filmo puslapį su atsiliepimais
-   **Sukurti, redaguoti, ištrinti atsiliepimą**
-   **Pateikti įkėlimui, redaguoti, ištrinti filmą**
-   **Pateikti, redaguoti, ištrinti reitingą**
-   Redaguoti savo įkeltus filmus
-   Peržiūrėti ir redaguoti profilį
-   Peržiūrėti parašytus atsiliepimus ir pateiktus filmų reitingus

**Administratorius** gali:

-   Patvirtinti naujų filmų įkėlimus
-   Peržiūrėti, įkelti, redaguoti, ištrinti filmų įrašus
-   Sukurti kategorijas filmams
-   Peržiūrėti, redaguoti, ištrinti atsiliepimus

## 2. Sistemos architektūra

Sistemos dalys:

-   **Front‑End**: React, TypeScript
-   **Back‑End**: ASP.NET Core (C#), REST API
-   **Duomenų bazė**: MySQL

Diegimas: naudojamas **Linux Ubuntu** serveris, visos dalys diegiamos tame pačiame serveryje. Aplikacija pasiekiama per **HTTPS**. **Bdmi API** vykdo duomenų mainus su DB per **ORM** sąsają (duomenų manipuliavimas ir tiekimas kliento sąsajai).

---

## OpenAPI specifikacija (JSON)

Žemiau pridėta automatiškai sugeneruota OpenAPI 3.0 specifikacija, aprašanti REST API maršrutus, parametrus, užklausų/atsakymų schemas, bei galimus atsakymų kodus.

```json
{
	"openapi": "3.0.4",
	"info": {
		"title": "bdmI API",
		"version": "v1"
	},
	"paths": {
		"/api/genres": {
			"get": {
				"tags": ["Genres"],
				"parameters": [
					{
						"name": "q",
						"in": "query",
						"schema": {
							"type": "string"
						}
					}
				],
				"responses": {
					"200": {
						"description": "OK",
						"content": {
							"application/json": {
								"schema": {
									"type": "array",
									"items": {
										"$ref": "#/components/schemas/GenreListItemDto"
									}
								}
							}
						}
					}
				}
			},
			"post": {
				"tags": ["Genres"],
				"requestBody": {
					"content": {
						"application/json": {
							"schema": {
								"$ref": "#/components/schemas/CreateGenreDto"
							}
						},
						"text/json": {
							"schema": {
								"$ref": "#/components/schemas/CreateGenreDto"
							}
						},
						"application/*+json": {
							"schema": {
								"$ref": "#/components/schemas/CreateGenreDto"
							}
						}
					}
				},
				"responses": {
					"201": {
						"description": "Created",
						"content": {
							"application/json": {
								"schema": {
									"$ref": "#/components/schemas/GenreDetailsDto"
								}
							}
						}
					},
					"400": {
						"description": "Bad Request",
						"content": {
							"application/json": {
								"schema": {
									"$ref": "#/components/schemas/ProblemDetails"
								}
							}
						}
					},
					"409": {
						"description": "Conflict",
						"content": {
							"application/json": {
								"schema": {
									"$ref": "#/components/schemas/ProblemDetails"
								}
							}
						}
					}
				}
			}
		},
		"/api/genres/{id}": {
			"get": {
				"tags": ["Genres"],
				"parameters": [
					{
						"name": "id",
						"in": "path",
						"required": true,
						"schema": {
							"type": "integer",
							"format": "int32"
						}
					}
				],
				"responses": {
					"200": {
						"description": "OK",
						"content": {
							"application/json": {
								"schema": {
									"$ref": "#/components/schemas/GenreDetailsDto"
								}
							}
						}
					},
					"404": {
						"description": "Not Found",
						"content": {
							"application/json": {
								"schema": {
									"$ref": "#/components/schemas/ProblemDetails"
								}
							}
						}
					}
				}
			},
			"put": {
				"tags": ["Genres"],
				"parameters": [
					{
						"name": "id",
						"in": "path",
						"required": true,
						"schema": {
							"type": "integer",
							"format": "int32"
						}
					}
				],
				"requestBody": {
					"content": {
						"application/json": {
							"schema": {
								"$ref": "#/components/schemas/UpdateGenreDto"
							}
						},
						"text/json": {
							"schema": {
								"$ref": "#/components/schemas/UpdateGenreDto"
							}
						},
						"application/*+json": {
							"schema": {
								"$ref": "#/components/schemas/UpdateGenreDto"
							}
						}
					}
				},
				"responses": {
					"204": {
						"description": "No Content"
					},
					"404": {
						"description": "Not Found",
						"content": {
							"application/json": {
								"schema": {
									"$ref": "#/components/schemas/ProblemDetails"
								}
							}
						}
					},
					"409": {
						"description": "Conflict",
						"content": {
							"application/json": {
								"schema": {
									"$ref": "#/components/schemas/ProblemDetails"
								}
							}
						}
					}
				}
			},
			"delete": {
				"tags": ["Genres"],
				"parameters": [
					{
						"name": "id",
						"in": "path",
						"required": true,
						"schema": {
							"type": "integer",
							"format": "int32"
						}
					}
				],
				"responses": {
					"204": {
						"description": "No Content"
					},
					"404": {
						"description": "Not Found",
						"content": {
							"application/json": {
								"schema": {
									"$ref": "#/components/schemas/ProblemDetails"
								}
							}
						}
					}
				}
			}
		},
		"/api/movies": {
			"get": {
				"tags": ["Movies"],
				"parameters": [
					{
						"name": "genreId",
						"in": "query",
						"schema": {
							"type": "integer",
							"format": "int32"
						}
					},
					{
						"name": "q",
						"in": "query",
						"schema": {
							"type": "string"
						}
					}
				],
				"responses": {
					"200": {
						"description": "OK",
						"content": {
							"application/json": {
								"schema": {
									"type": "array",
									"items": {
										"$ref": "#/components/schemas/MovieListItemDto"
									}
								}
							}
						}
					}
				}
			},
			"post": {
				"tags": ["Movies"],
				"requestBody": {
					"content": {
						"application/json": {
							"schema": {
								"$ref": "#/components/schemas/CreateMovieDto"
							}
						},
						"text/json": {
							"schema": {
								"$ref": "#/components/schemas/CreateMovieDto"
							}
						},
						"application/*+json": {
							"schema": {
								"$ref": "#/components/schemas/CreateMovieDto"
							}
						}
					}
				},
				"responses": {
					"201": {
						"description": "Created",
						"content": {
							"application/json": {
								"schema": {
									"$ref": "#/components/schemas/MovieDetailsDto"
								}
							}
						}
					},
					"400": {
						"description": "Bad Request",
						"content": {
							"application/json": {
								"schema": {
									"$ref": "#/components/schemas/ProblemDetails"
								}
							}
						}
					},
					"409": {
						"description": "Conflict",
						"content": {
							"application/json": {
								"schema": {
									"$ref": "#/components/schemas/ProblemDetails"
								}
							}
						}
					},
					"422": {
						"description": "Unprocessable Content",
						"content": {
							"application/json": {
								"schema": {
									"$ref": "#/components/schemas/ProblemDetails"
								}
							}
						}
					}
				}
			}
		},
		"/api/movies/{id}": {
			"get": {
				"tags": ["Movies"],
				"parameters": [
					{
						"name": "id",
						"in": "path",
						"required": true,
						"schema": {
							"type": "integer",
							"format": "int32"
						}
					}
				],
				"responses": {
					"200": {
						"description": "OK",
						"content": {
							"application/json": {
								"schema": {
									"$ref": "#/components/schemas/MovieDetailsDto"
								}
							}
						}
					},
					"404": {
						"description": "Not Found",
						"content": {
							"application/json": {
								"schema": {
									"$ref": "#/components/schemas/ProblemDetails"
								}
							}
						}
					}
				}
			},
			"put": {
				"tags": ["Movies"],
				"parameters": [
					{
						"name": "id",
						"in": "path",
						"required": true,
						"schema": {
							"type": "integer",
							"format": "int32"
						}
					}
				],
				"requestBody": {
					"content": {
						"application/json": {
							"schema": {
								"$ref": "#/components/schemas/UpdateMovieDto"
							}
						},
						"text/json": {
							"schema": {
								"$ref": "#/components/schemas/UpdateMovieDto"
							}
						},
						"application/*+json": {
							"schema": {
								"$ref": "#/components/schemas/UpdateMovieDto"
							}
						}
					}
				},
				"responses": {
					"204": {
						"description": "No Content"
					},
					"404": {
						"description": "Not Found",
						"content": {
							"application/json": {
								"schema": {
									"$ref": "#/components/schemas/ProblemDetails"
								}
							}
						}
					},
					"409": {
						"description": "Conflict",
						"content": {
							"application/json": {
								"schema": {
									"$ref": "#/components/schemas/ProblemDetails"
								}
							}
						}
					},
					"422": {
						"description": "Unprocessable Content",
						"content": {
							"application/json": {
								"schema": {
									"$ref": "#/components/schemas/ProblemDetails"
								}
							}
						}
					}
				}
			},
			"delete": {
				"tags": ["Movies"],
				"parameters": [
					{
						"name": "id",
						"in": "path",
						"required": true,
						"schema": {
							"type": "integer",
							"format": "int32"
						}
					}
				],
				"responses": {
					"204": {
						"description": "No Content"
					},
					"404": {
						"description": "Not Found",
						"content": {
							"application/json": {
								"schema": {
									"$ref": "#/components/schemas/ProblemDetails"
								}
							}
						}
					}
				}
			}
		},
		"/api/movies/{id}/reviews": {
			"get": {
				"tags": ["Movies"],
				"parameters": [
					{
						"name": "id",
						"in": "path",
						"required": true,
						"schema": {
							"type": "integer",
							"format": "int32"
						}
					},
					{
						"name": "includeText",
						"in": "query",
						"schema": {
							"type": "boolean",
							"default": false
						}
					}
				],
				"responses": {
					"200": {
						"description": "OK",
						"content": {
							"application/json": {
								"schema": {
									"$ref": "#/components/schemas/MovieReviewsDto"
								}
							}
						}
					},
					"404": {
						"description": "Not Found",
						"content": {
							"application/json": {
								"schema": {
									"$ref": "#/components/schemas/ProblemDetails"
								}
							}
						}
					}
				}
			}
		},
		"/api/reviews": {
			"get": {
				"tags": ["Reviews"],
				"parameters": [
					{
						"name": "movieId",
						"in": "query",
						"schema": {
							"type": "integer",
							"format": "int32"
						}
					},
					{
						"name": "userId",
						"in": "query",
						"schema": {
							"type": "integer",
							"format": "int32"
						}
					}
				],
				"responses": {
					"200": {
						"description": "OK",
						"content": {
							"application/json": {
								"schema": {
									"type": "array",
									"items": {
										"$ref": "#/components/schemas/ReviewListItemDto"
									}
								}
							}
						}
					}
				}
			},
			"post": {
				"tags": ["Reviews"],
				"requestBody": {
					"content": {
						"application/json": {
							"schema": {
								"$ref": "#/components/schemas/CreateReviewDto"
							}
						},
						"text/json": {
							"schema": {
								"$ref": "#/components/schemas/CreateReviewDto"
							}
						},
						"application/*+json": {
							"schema": {
								"$ref": "#/components/schemas/CreateReviewDto"
							}
						}
					}
				},
				"responses": {
					"201": {
						"description": "Created",
						"content": {
							"application/json": {
								"schema": {
									"$ref": "#/components/schemas/ReviewDetailsDto"
								}
							}
						}
					},
					"400": {
						"description": "Bad Request",
						"content": {
							"application/json": {
								"schema": {
									"$ref": "#/components/schemas/ProblemDetails"
								}
							}
						}
					},
					"409": {
						"description": "Conflict",
						"content": {
							"application/json": {
								"schema": {
									"$ref": "#/components/schemas/ProblemDetails"
								}
							}
						}
					},
					"422": {
						"description": "Unprocessable Content",
						"content": {
							"application/json": {
								"schema": {
									"$ref": "#/components/schemas/ProblemDetails"
								}
							}
						}
					}
				}
			}
		},
		"/api/reviews/{id}": {
			"get": {
				"tags": ["Reviews"],
				"parameters": [
					{
						"name": "id",
						"in": "path",
						"required": true,
						"schema": {
							"type": "integer",
							"format": "int32"
						}
					}
				],
				"responses": {
					"200": {
						"description": "OK",
						"content": {
							"application/json": {
								"schema": {
									"$ref": "#/components/schemas/ReviewDetailsDto"
								}
							}
						}
					},
					"404": {
						"description": "Not Found",
						"content": {
							"application/json": {
								"schema": {
									"$ref": "#/components/schemas/ProblemDetails"
								}
							}
						}
					}
				}
			},
			"put": {
				"tags": ["Reviews"],
				"parameters": [
					{
						"name": "id",
						"in": "path",
						"required": true,
						"schema": {
							"type": "integer",
							"format": "int32"
						}
					}
				],
				"requestBody": {
					"content": {
						"application/json": {
							"schema": {
								"$ref": "#/components/schemas/UpdateReviewDto"
							}
						},
						"text/json": {
							"schema": {
								"$ref": "#/components/schemas/UpdateReviewDto"
							}
						},
						"application/*+json": {
							"schema": {
								"$ref": "#/components/schemas/UpdateReviewDto"
							}
						}
					}
				},
				"responses": {
					"204": {
						"description": "No Content"
					},
					"404": {
						"description": "Not Found",
						"content": {
							"application/json": {
								"schema": {
									"$ref": "#/components/schemas/ProblemDetails"
								}
							}
						}
					}
				}
			},
			"delete": {
				"tags": ["Reviews"],
				"parameters": [
					{
						"name": "id",
						"in": "path",
						"required": true,
						"schema": {
							"type": "integer",
							"format": "int32"
						}
					}
				],
				"responses": {
					"204": {
						"description": "No Content"
					},
					"404": {
						"description": "Not Found",
						"content": {
							"application/json": {
								"schema": {
									"$ref": "#/components/schemas/ProblemDetails"
								}
							}
						}
					}
				}
			}
		},
		"/api/users": {
			"get": {
				"tags": ["Users"],
				"parameters": [
					{
						"name": "q",
						"in": "query",
						"schema": {
							"type": "string"
						}
					}
				],
				"responses": {
					"200": {
						"description": "OK",
						"content": {
							"application/json": {
								"schema": {
									"type": "array",
									"items": {
										"$ref": "#/components/schemas/UserListItemDto"
									}
								}
							}
						}
					}
				}
			},
			"post": {
				"tags": ["Users"],
				"requestBody": {
					"content": {
						"application/json": {
							"schema": {
								"$ref": "#/components/schemas/CreateUserDto"
							}
						},
						"text/json": {
							"schema": {
								"$ref": "#/components/schemas/CreateUserDto"
							}
						},
						"application/*+json": {
							"schema": {
								"$ref": "#/components/schemas/CreateUserDto"
							}
						}
					}
				},
				"responses": {
					"201": {
						"description": "Created",
						"content": {
							"application/json": {
								"schema": {
									"$ref": "#/components/schemas/UserDetailsDto"
								}
							}
						}
					},
					"400": {
						"description": "Bad Request",
						"content": {
							"application/json": {
								"schema": {
									"$ref": "#/components/schemas/ProblemDetails"
								}
							}
						}
					},
					"409": {
						"description": "Conflict",
						"content": {
							"application/json": {
								"schema": {
									"$ref": "#/components/schemas/ProblemDetails"
								}
							}
						}
					}
				}
			}
		},
		"/api/users/{id}": {
			"get": {
				"tags": ["Users"],
				"parameters": [
					{
						"name": "id",
						"in": "path",
						"required": true,
						"schema": {
							"type": "integer",
							"format": "int32"
						}
					}
				],
				"responses": {
					"200": {
						"description": "OK",
						"content": {
							"application/json": {
								"schema": {
									"$ref": "#/components/schemas/UserDetailsDto"
								}
							}
						}
					},
					"404": {
						"description": "Not Found",
						"content": {
							"application/json": {
								"schema": {
									"$ref": "#/components/schemas/ProblemDetails"
								}
							}
						}
					}
				}
			},
			"put": {
				"tags": ["Users"],
				"parameters": [
					{
						"name": "id",
						"in": "path",
						"required": true,
						"schema": {
							"type": "integer",
							"format": "int32"
						}
					}
				],
				"requestBody": {
					"content": {
						"application/json": {
							"schema": {
								"$ref": "#/components/schemas/UpdateUserDto"
							}
						},
						"text/json": {
							"schema": {
								"$ref": "#/components/schemas/UpdateUserDto"
							}
						},
						"application/*+json": {
							"schema": {
								"$ref": "#/components/schemas/UpdateUserDto"
							}
						}
					}
				},
				"responses": {
					"204": {
						"description": "No Content"
					},
					"404": {
						"description": "Not Found",
						"content": {
							"application/json": {
								"schema": {
									"$ref": "#/components/schemas/ProblemDetails"
								}
							}
						}
					},
					"409": {
						"description": "Conflict",
						"content": {
							"application/json": {
								"schema": {
									"$ref": "#/components/schemas/ProblemDetails"
								}
							}
						}
					}
				}
			},
			"delete": {
				"tags": ["Users"],
				"parameters": [
					{
						"name": "id",
						"in": "path",
						"required": true,
						"schema": {
							"type": "integer",
							"format": "int32"
						}
					}
				],
				"responses": {
					"204": {
						"description": "No Content"
					},
					"404": {
						"description": "Not Found",
						"content": {
							"application/json": {
								"schema": {
									"$ref": "#/components/schemas/ProblemDetails"
								}
							}
						}
					},
					"409": {
						"description": "Conflict",
						"content": {
							"application/json": {
								"schema": {
									"$ref": "#/components/schemas/ProblemDetails"
								}
							}
						}
					}
				}
			}
		}
	},
	"components": {
		"schemas": {
			"CreateGenreDto": {
				"required": ["name"],
				"type": "object",
				"properties": {
					"name": {
						"maxLength": 50,
						"minLength": 0,
						"type": "string"
					},
					"description": {
						"maxLength": 500,
						"minLength": 0,
						"type": "string",
						"nullable": true
					}
				},
				"additionalProperties": false
			},
			"CreateMovieDto": {
				"required": ["description", "title", "uploadedByUserId"],
				"type": "object",
				"properties": {
					"title": {
						"maxLength": 200,
						"minLength": 0,
						"type": "string"
					},
					"description": {
						"maxLength": 1000,
						"minLength": 0,
						"type": "string"
					},
					"releaseYear": {
						"maximum": 2100,
						"minimum": 1888,
						"type": "integer",
						"format": "int32"
					},
					"durationMin": {
						"maximum": 600,
						"minimum": 1,
						"type": "integer",
						"format": "int32"
					},
					"uploadedByUserId": {
						"type": "integer",
						"format": "int32"
					},
					"genreIds": {
						"type": "array",
						"items": {
							"type": "integer",
							"format": "int32"
						},
						"nullable": true
					}
				},
				"additionalProperties": false
			},
			"CreateReviewDto": {
				"required": ["movieId", "userId"],
				"type": "object",
				"properties": {
					"movieId": {
						"type": "integer",
						"format": "int32"
					},
					"userId": {
						"type": "integer",
						"format": "int32"
					},
					"score": {
						"maximum": 10,
						"minimum": 1,
						"type": "integer",
						"format": "int32"
					},
					"title": {
						"maxLength": 120,
						"minLength": 0,
						"type": "string",
						"nullable": true
					},
					"body": {
						"maxLength": 4000,
						"minLength": 0,
						"type": "string",
						"nullable": true
					}
				},
				"additionalProperties": false
			},
			"CreateUserDto": {
				"required": ["email", "username"],
				"type": "object",
				"properties": {
					"username": {
						"maxLength": 40,
						"minLength": 0,
						"type": "string"
					},
					"email": {
						"maxLength": 256,
						"minLength": 0,
						"type": "string",
						"format": "email"
					}
				},
				"additionalProperties": false
			},
			"GenreDetailsDto": {
				"type": "object",
				"properties": {
					"id": {
						"type": "integer",
						"format": "int32"
					},
					"name": {
						"type": "string",
						"nullable": true
					}
				},
				"additionalProperties": false
			},
			"GenreListItemDto": {
				"type": "object",
				"properties": {
					"id": {
						"type": "integer",
						"format": "int32"
					},
					"name": {
						"type": "string",
						"nullable": true
					}
				},
				"additionalProperties": false
			},
			"MovieDetailsDto": {
				"type": "object",
				"properties": {
					"id": {
						"type": "integer",
						"format": "int32"
					},
					"title": {
						"type": "string",
						"nullable": true
					},
					"description": {
						"type": "string",
						"nullable": true
					},
					"releaseYear": {
						"type": "integer",
						"format": "int32"
					},
					"durationMin": {
						"type": "integer",
						"format": "int32"
					},
					"uploadedByUserId": {
						"type": "integer",
						"format": "int32"
					},
					"genreIds": {
						"type": "array",
						"items": {
							"type": "integer",
							"format": "int32"
						},
						"nullable": true
					},
					"averageScore": {
						"type": "number",
						"format": "double"
					}
				},
				"additionalProperties": false
			},
			"MovieListItemDto": {
				"type": "object",
				"properties": {
					"id": {
						"type": "integer",
						"format": "int32"
					},
					"title": {
						"type": "string",
						"nullable": true
					},
					"releaseYear": {
						"type": "integer",
						"format": "int32"
					},
					"durationMin": {
						"type": "integer",
						"format": "int32"
					},
					"genres": {
						"type": "array",
						"items": {
							"type": "string"
						},
						"nullable": true
					},
					"averageScore": {
						"type": "number",
						"format": "double"
					}
				},
				"additionalProperties": false
			},
			"MovieReviewsDto": {
				"type": "object",
				"properties": {
					"movieId": {
						"type": "integer",
						"format": "int32"
					},
					"reviews": {
						"type": "array",
						"items": {
							"$ref": "#/components/schemas/ReviewForMovieDto"
						},
						"nullable": true
					}
				},
				"additionalProperties": false
			},
			"ProblemDetails": {
				"type": "object",
				"properties": {
					"type": {
						"type": "string",
						"nullable": true
					},
					"title": {
						"type": "string",
						"nullable": true
					},
					"status": {
						"type": "integer",
						"format": "int32",
						"nullable": true
					},
					"detail": {
						"type": "string",
						"nullable": true
					},
					"instance": {
						"type": "string",
						"nullable": true
					}
				},
				"additionalProperties": {}
			},
			"ReviewDetailsDto": {
				"type": "object",
				"properties": {
					"id": {
						"type": "integer",
						"format": "int32"
					},
					"movieId": {
						"type": "integer",
						"format": "int32"
					},
					"userId": {
						"type": "integer",
						"format": "int32"
					},
					"score": {
						"type": "integer",
						"format": "int32"
					},
					"title": {
						"type": "string",
						"nullable": true
					},
					"body": {
						"type": "string",
						"nullable": true
					},
					"createdAt": {
						"type": "string",
						"format": "date-time"
					}
				},
				"additionalProperties": false
			},
			"ReviewForMovieDto": {
				"type": "object",
				"properties": {
					"id": {
						"type": "integer",
						"format": "int32"
					},
					"userId": {
						"type": "integer",
						"format": "int32"
					},
					"score": {
						"type": "integer",
						"format": "int32"
					},
					"title": {
						"type": "string",
						"nullable": true
					},
					"body": {
						"type": "string",
						"nullable": true
					},
					"createdAt": {
						"type": "string",
						"format": "date-time"
					}
				},
				"additionalProperties": false
			},
			"ReviewListItemDto": {
				"type": "object",
				"properties": {
					"id": {
						"type": "integer",
						"format": "int32"
					},
					"movieId": {
						"type": "integer",
						"format": "int32"
					},
					"userId": {
						"type": "integer",
						"format": "int32"
					},
					"score": {
						"type": "integer",
						"format": "int32"
					},
					"title": {
						"type": "string",
						"nullable": true
					},
					"createdAt": {
						"type": "string",
						"format": "date-time"
					}
				},
				"additionalProperties": false
			},
			"UpdateGenreDto": {
				"required": ["name"],
				"type": "object",
				"properties": {
					"name": {
						"maxLength": 50,
						"minLength": 0,
						"type": "string"
					},
					"description": {
						"maxLength": 500,
						"minLength": 0,
						"type": "string",
						"nullable": true
					}
				},
				"additionalProperties": false
			},
			"UpdateMovieDto": {
				"required": ["description", "title"],
				"type": "object",
				"properties": {
					"title": {
						"maxLength": 200,
						"minLength": 0,
						"type": "string"
					},
					"description": {
						"maxLength": 1000,
						"minLength": 0,
						"type": "string"
					},
					"releaseYear": {
						"maximum": 2100,
						"minimum": 1888,
						"type": "integer",
						"format": "int32"
					},
					"durationMin": {
						"maximum": 600,
						"minimum": 1,
						"type": "integer",
						"format": "int32"
					},
					"genreIds": {
						"type": "array",
						"items": {
							"type": "integer",
							"format": "int32"
						},
						"nullable": true
					}
				},
				"additionalProperties": false
			},
			"UpdateReviewDto": {
				"type": "object",
				"properties": {
					"score": {
						"maximum": 10,
						"minimum": 1,
						"type": "integer",
						"format": "int32"
					},
					"title": {
						"maxLength": 120,
						"minLength": 0,
						"type": "string",
						"nullable": true
					},
					"body": {
						"maxLength": 4000,
						"minLength": 0,
						"type": "string",
						"nullable": true
					}
				},
				"additionalProperties": false
			},
			"UpdateUserDto": {
				"required": ["email", "username"],
				"type": "object",
				"properties": {
					"username": {
						"maxLength": 40,
						"minLength": 0,
						"type": "string"
					},
					"email": {
						"maxLength": 256,
						"minLength": 0,
						"type": "string",
						"format": "email"
					}
				},
				"additionalProperties": false
			},
			"UserDetailsDto": {
				"type": "object",
				"properties": {
					"id": {
						"type": "integer",
						"format": "int32"
					},
					"username": {
						"type": "string",
						"nullable": true
					},
					"email": {
						"type": "string",
						"nullable": true
					},
					"createdAt": {
						"type": "string",
						"format": "date-time"
					}
				},
				"additionalProperties": false
			},
			"UserListItemDto": {
				"type": "object",
				"properties": {
					"id": {
						"type": "integer",
						"format": "int32"
					},
					"username": {
						"type": "string",
						"nullable": true
					},
					"email": {
						"type": "string",
						"nullable": true
					},
					"createdAt": {
						"type": "string",
						"format": "date-time"
					}
				},
				"additionalProperties": false
			}
		}
	}
}
```
